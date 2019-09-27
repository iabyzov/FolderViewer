using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebHost.Infrasctructure.ExceptionHandling
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IServiceProvider _serviceProvider;

        public ExceptionHandlingMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            this.next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (PublicException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (OperationCanceledException ex)
            {
                HandleCanceledException(context, ex);
            }
        }

        private void HandleCanceledException(HttpContext context,
            OperationCanceledException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }

        private Task HandleExceptionAsync(HttpContext context, PublicException exception)
        {
            var errorProvider = ResolveErrorResponseProvider(exception.GetType());
            if (errorProvider == null)
            {
                return Task.CompletedTask;
            }

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            var result = JsonConvert.SerializeObject(errorProvider.BuildErrorMessage(exception), serializerSettings);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)errorProvider.StatusCode;

            return context.Response.WriteAsync(result);
        }

        private IErrorReponseProvider ResolveErrorResponseProvider(Type exceptionType)
        {
            var errorProviders = _serviceProvider.GetServices<IErrorReponseProvider>();
            var foundProviders = errorProviders.Where(x => x.HandledExceptionType == exceptionType).ToList();

            if (foundProviders.Count == 0)
            {
                return null;
            }

            if (foundProviders.Count > 1)
            {
                throw new InvalidOperationException($"Found more than 1 provider for {exceptionType}");
            }

            return foundProviders.Single();
        }
    }
}