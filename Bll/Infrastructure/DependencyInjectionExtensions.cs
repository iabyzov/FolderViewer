using Bll.Queries.Folder;
using Microsoft.Extensions.DependencyInjection;

namespace Bll.Infrastructure
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddBllDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IFileProviderFactory, FileProviderFactory>();

            return serviceCollection;
        }
    }
}