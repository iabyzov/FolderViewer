using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Exceptions;
using Common.Utils;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;

namespace Bll.Queries.Folder
{
    class GetSortedFoldersQueryHandler : IRequestHandler<GetSortedFoldersDefinition, IEnumerable<FolderInfo>>
    {
        private readonly IFileProviderFactory _fileProviderFactory;
        private readonly IMemoryCache _memoryCache;

        public GetSortedFoldersQueryHandler(IFileProviderFactory fileProviderFactory, IMemoryCache memoryCache)
        {
            Guard.IsNotNull(fileProviderFactory, nameof(fileProviderFactory));
            Guard.IsNotNull(memoryCache, nameof(memoryCache));
            _fileProviderFactory = fileProviderFactory;
            _memoryCache = memoryCache;
        }

        public Task<IEnumerable<FolderInfo>> Handle(GetSortedFoldersDefinition parameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Validate(_fileProviderFactory, parameters);

            if (parameters.Limit == 0)
            {
                return Task.FromResult(Enumerable.Empty<FolderInfo>());
            }

            if (_memoryCache.TryGetValue(parameters.Path, out var cacheResult))
            {
                return Task.FromResult((IEnumerable<FolderInfo>) cacheResult);
            }

            var result = GetFolderInfosInternal(parameters, _fileProviderFactory, cancellationToken);
            _memoryCache.Set(parameters.Path, result, TimeSpan.FromMinutes(5));
            return Task.FromResult(result.AsEnumerable());
        }

        private static FolderInfo[] GetFolderInfosInternal(GetSortedFoldersDefinition parameters, IFileProviderFactory fileProviderFactory,  CancellationToken cancellationToken)
        {
            var fileProvider = fileProviderFactory.GetFileProvider(parameters.Path);

            var rootContents = fileProvider.GetDirectoryContents(string.Empty);
            var topDirs = rootContents.Where(fi => fi.IsDirectory);

            var resultList = new List<FolderInfo>();
            foreach (var topDir in topDirs)
            {
                var size = GetFolderSize(fileProvider, topDir, cancellationToken);
                var folderInfo = new FolderInfo {Name = topDir.Name, Size = size};
                resultList.Add(folderInfo);
            }

            var orderedFolders = resultList.OrderByDescending(f => f.Size);
            var result =  parameters.Limit.HasValue ? orderedFolders.Take(parameters.Limit.Value) : orderedFolders;
            return result.ToArray();
        }

        private static void Validate(IFileProviderFactory fileProviderFactory, GetSortedFoldersDefinition parameters)
        {
            if (parameters.Limit < 0)
            {
                throw new ValidationPublicException("Negative value can't be used for a limit");
            }
            if (!fileProviderFactory.Exists(parameters.Path))
            {
                throw new ObjectNotFoundPublicException("Path not found");
            }
        }

        private static long GetFolderSize(IFileProvider fileProvider, IFileInfo path, CancellationToken cancellationToken)
        {
            var queue = new Queue<string>();
            queue.Enqueue(path.Name);
            long result = 0;

            while (queue.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var item = queue.Dequeue();
                var contents = fileProvider.GetDirectoryContents(item);

                result += contents.Where(fi => !fi.IsDirectory).Sum(fileInfo => fileInfo.Length);

                var subFolders = contents.Where(fi => fi.IsDirectory);
                foreach (var directoryInfo in subFolders)
                {
                    queue.Enqueue(Path.Combine(item, directoryInfo.Name));
                }
            }

            return result;
        }
    }
}
