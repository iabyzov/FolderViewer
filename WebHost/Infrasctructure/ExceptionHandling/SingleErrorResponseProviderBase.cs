using System.Collections.Generic;
using Common.Exceptions;
using WebHost.ClientApi.Errors;

namespace WebHost.Infrasctructure.ExceptionHandling
{
    public abstract class SingleErrorResponseProviderBase<TException> : ErrorResponseProviderBase<TException>
        where TException : PublicException
    {
        protected override ApiError CreateContent(TException exception)
        {
            return new ApiError
            {
                Errors = new List<Error>
                {
                    new Error(exception.Message)
                }
            };
        }
    }
}