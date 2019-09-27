using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Bll.Queries.Folder
{
    class FileProviderFactory : IFileProviderFactory
    {
        public IFileProvider GetFileProvider(string path)
        {
            return new PhysicalFileProvider(path);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }
    }
}