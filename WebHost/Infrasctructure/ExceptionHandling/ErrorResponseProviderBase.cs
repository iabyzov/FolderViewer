using System;
using System.Net;
using Common.Exceptions;
using Common.Utils;
using Microsoft.EntityFrameworkCore.Internal;
using WebHost.ClientApi.Errors;

namespace WebHost.Infrasctructure.ExceptionHandling
{
    public abstract class ErrorResponseProviderBase<TException> : IErrorReponseProvider where TException : PublicException
    {
        public Type HandledExceptionType { get; } = typeof(TException);

        public ApiError BuildErrorMessage(PublicException exception)
        {
            Guard.IsNotNull(exception, nameof(exception));

            if (exception is TException typedException)
            {
                return BuildErrorMessageInternal(typedException);
            }

            throw new ArgumentException($"Not expected exception. Expected type was {HandledExceptionType} but origin is {exception.GetType()}");
        }

        public abstract HttpStatusCode StatusCode { get; }

        protected abstract ApiError CreateContent(TException exception);

        private ApiError BuildErrorMessageInternal(TException exception)
        {
            var error = CreateContent(exception);
            return error;
        }
    }
}