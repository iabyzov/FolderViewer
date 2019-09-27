using System;
using System.Net;
using System.Net.Http;
using Common.Exceptions;
using WebHost.ClientApi.Errors;

namespace WebHost.Infrasctructure.ExceptionHandling
{
    public interface IErrorReponseProvider
    {
        Type HandledExceptionType { get; }

        HttpStatusCode StatusCode { get; }

        ApiError BuildErrorMessage(PublicException exception);
    }
}