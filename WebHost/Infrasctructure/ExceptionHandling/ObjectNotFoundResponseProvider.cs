using System.Net;
using Common.Exceptions;

namespace WebHost.Infrasctructure.ExceptionHandling
{
    public class ObjectNotFoundResponseProvider : SingleErrorResponseProviderBase<ObjectNotFoundPublicException>
    {
        public override HttpStatusCode StatusCode { get; } = HttpStatusCode.NotFound;
    }
}