using MediatR;
using Microsoft.Extensions.DependencyInjection;
using WebHost.Infrasctructure.ExceptionHandling;

namespace WebHost.Infrasctructure.Dependency
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddHostDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IErrorReponseProvider, ObjectNotFoundResponseProvider>();
            serviceCollection.AddSingleton<IErrorReponseProvider, ValidationErrorResponseProvider>();

            return serviceCollection;
        }
    }
}