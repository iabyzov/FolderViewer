using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bll.Queries.Folder;
using Common.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Moq;
using NUnit.Framework;

namespace Bll.Tests.Queries.Folder
{
    public class GetSortedFoldersQueryHandlerTests
    {
        private GetSortedFoldersQueryHandler _handler;
        private GetSortedFoldersDefinition _definition;
        private Mock<IFileProvider> _fileProviderMock;
        private Dictionary<string, IDirectoryContents> _pathToContentMap;
        private Mock<IFileProviderFactory> _fileProviderFactoryMock;
        private Mock<IMemoryCache> _memoryCacheMock;

        [SetUp]
        public void Setup()
        {
            _fileProviderMock = new Mock<IFileProvider>();
            _fileProviderMock.Setup(x => x.GetDirectoryContents(It.IsAny<string>()))
                .Returns((string subPath) =>  _pathToContentMap[subPath]);

            _fileProviderFactoryMock = new Mock<IFileProviderFactory>();
            _fileProviderFactoryMock.Setup(x => x.GetFileProvider(It.IsAny<string>())).Returns(_fileProviderMock.Object);
            _fileProviderFactoryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            _memoryCacheMock = new Mock<IMemoryCache>();
            var cacheObjectMock = new Mock<ICacheEntry>();
            _memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheObjectMock.Object);

            _handler = new GetSortedFoldersQueryHandler(_fileProviderFactoryMock.Object, _memoryCacheMock.Object);
            _definition = new GetSortedFoldersDefinition();

            ProcessNodes(new FolderNode("root"));
        }

        private void ProcessNodes(FolderNode rootNode)
        {
            _pathToContentMap = new Dictionary<string, IDirectoryContents>();
            ProcessNodesBacktracking(rootNode, string.Empty);
        }

        private void ProcessNodesBacktracking(FolderNode currentNode, string path)
        {
            if (currentNode == null)
            {
                return;
            }

            var result = new List<IFileInfo>();
            var files = currentNode.Nodes.OfType<FileNode>().Select(fn => CreateFileMock(fn.Length));
            result.AddRange(files);
            var folders = currentNode.Nodes.OfType<FolderNode>().ToArray();
            result.AddRange(folders.Select(fn => CreateDirectoryMock(fn.Name)));
            _pathToContentMap[path] = CreateDirectoryContentsMock(result);

            foreach (var folder in folders)
            {
                ProcessNodesBacktracking(folder, Path.Combine(path, folder.Name));
            }
        }

        private IDirectoryContents CreateDirectoryContentsMock(IEnumerable<IFileInfo> files)
        {
            var directoryMock = new Mock<IDirectoryContents>();
            directoryMock.Setup(x => x.GetEnumerator()).Returns(files.GetEnumerator);
            return directoryMock.Object;
        }

        private IFileInfo CreateDirectoryMock(string name)
        {
            var directoryMock = new Mock<IFileInfo>();
            directoryMock.Setup(x => x.IsDirectory).Returns(true);
            directoryMock.Setup(x => x.Name).Returns(name);
            return directoryMock.Object;
        }

        private IFileInfo CreateFileMock(long length)
        {
            var fileMock = new Mock<IFileInfo>();
            fileMock.Setup(x => x.IsDirectory).Returns(false);
            fileMock.Setup(x => x.Length).Returns(length);
            return fileMock.Object;
        }

        [Test]
        public void FolderNotFound_ThrowsException()
        {
            _fileProviderFactoryMock.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);

            async Task Act() => await ExecuteHandler();

            Assert.ThrowsAsync<ObjectNotFoundPublicException>(Act);
        }

        [Test]
        public void LimitIsNegative_ThrowsException()
        {
            _definition.Limit = -1;

            async Task Act() => await ExecuteHandler();

            Assert.ThrowsAsync<ValidationPublicException>(Act);
        }

        [Test]
        public async Task RootFolderIsEmpty_EmptyResult()
        {
            var res = await ExecuteHandler();

            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public async Task LimitIsZero_FileProviderIsNotUsed()
        {
            _definition.Limit = 0;

            await ExecuteHandler();

            _fileProviderFactoryMock.Verify(x => x.GetFileProvider(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task RootFolderHasNoFolders_EmptyResult()
        {
            var rootNode = new FolderNode("root")
            {
                Nodes = new List<Node>
                {
                    new FileNode(2056),
                    new FileNode(12000)
                }
            };
            ProcessNodes(rootNode);

            var res = await ExecuteHandler();

            Assert.AreEqual(0, res.Count());
        }

        [Test]
        public async Task RootFolderHasOnlyEmptyFolders_EmptyFoldersReturned()
        {
            var rootNode = new FolderNode("root")
            {
                Nodes = new List<Node>
                {
                    new FileNode(2056),
                    new FileNode(12000),
                    new FolderNode("folder1"),
                    new FolderNode("folder2")
                }

            };
            ProcessNodes(rootNode);

            var res = await ExecuteHandler();

            Assert.AreEqual(2, res.Count());
            res.ToList().ForEach(r => Assert.AreEqual(0, r.Size));
        }

        [Test]
        public async Task OneFolderWithNestedFolders_CalculatesAllLevels()
        {
            var rootNode = new FolderNode("root")
            {
                Nodes = new List<Node>
                {
                    new FileNode(2056),
                    new FileNode(12000),
                    new FolderNode("level1")
                    {
                        Nodes = new List<Node>()
                        {
                            new FileNode(1),
                            new FolderNode("level2")
                            {
                                Nodes = new List<Node>()
                                {
                                    new FileNode(3),
                                    new FolderNode("level3")
                                    {
                                        Nodes = new List<Node>()
                                        {
                                            new FileNode(4),
                                            new FileNode(5)
                                        }
                                    }
                                }
                            },
                            new FileNode(2)
                        }
                    }
                }
            };
            ProcessNodes(rootNode);

            var res = await ExecuteHandler();

            Assert.AreEqual(1, res.Count());
            Assert.AreEqual(15, res.First().Size);
        }

        [Test]
        public async Task NotEqualFolders_FoldersOrderedBySize()
        {
            var rootNode = new FolderNode("root")
            {
                Nodes = new List<Node>
                {
                    new FolderNode("folder1")
                    {
                        Nodes = new List<Node>()
                        {
                            new FileNode(1),
                        }
                    },
                    new FolderNode("folder2")
                    {
                        Nodes = new List<Node>()
                        {
                            new FileNode(3)
                        }
                    },
                    new FolderNode("folder3")
                    {
                        Nodes = new List<Node>()
                        {
                            new FileNode(2)
                        }
                    }
                }
            };
            ProcessNodes(rootNode);

            var res = (await ExecuteHandler()).ToArray();

            Assert.AreEqual(3, res[0].Size);
            Assert.AreEqual(2, res[1].Size);
            Assert.AreEqual(1, res[2].Size);
        }

        // N > M
        [Test]
        public async Task NFolders_LimitedByM()
        {
            var rootNode = new FolderNode("root")
            {
                Nodes = new List<Node>
                {
                    new FolderNode("folder1"),
                    new FolderNode("folder2"),
                    new FolderNode("folder3"),
                }
            };
            ProcessNodes(rootNode);
            _definition.Limit = 2;

            var res = await ExecuteHandler();

            Assert.AreEqual(2, res.Count());
        }

        [Test]
        public async Task NFolders_LimitedByM_MFoldersReturned()
        {
            var rootNode = new FolderNode("root")
            {
                Nodes = new List<Node>
                {
                    new FolderNode("folder1"),
                    new FolderNode("folder2"),
                    new FolderNode("folder3"),
                }
            };
            ProcessNodes(rootNode);
            _definition.Limit = 2;

            var res = await ExecuteHandler();

            Assert.AreEqual(2, res.Count());
        }

        // N > M
        [Test]
        public async Task MFolders_LimitedByN_MReturned()
        {
            var rootNode = new FolderNode("root")
            {
                Nodes = new List<Node>
                {
                    new FolderNode("folder1"),
                    new FolderNode("folder2"),
                }
            };
            ProcessNodes(rootNode);
            _definition.Limit = 3;

            var res = await ExecuteHandler();

            Assert.AreEqual(2, res.Count());
        }

        private async Task<IEnumerable<FolderInfo>> ExecuteHandler()
        {
            return await _handler.Handle(_definition);
        }
    }
}