using Microsoft.Extensions.FileProviders;

namespace Bll.Queries.Folder
{
    public interface IFileProviderFactory
    {
        IFileProvider GetFileProvider(string path);
        bool Exists(string path);
    }
}