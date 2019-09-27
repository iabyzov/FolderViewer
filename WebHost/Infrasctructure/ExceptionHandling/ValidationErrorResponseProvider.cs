using System.Collections.Generic;
using System.Linq;
using System.Net;
using Common.Exceptions;
using WebHost.ClientApi.Errors;

namespace WebHost.Infrasctructure.ExceptionHandling
{
    public class ValidationErrorResponseProvider : ErrorResponseProviderBase<ValidationPublicException>
    {
        protected override ApiError CreateContent(ValidationPublicException exception)
        {
            var result = new ApiError { Errors = new List<Error>()};
            if (!exception.ValidationErrors.Any())
            {
                result.Errors.Add(new Error(exception.Message));
            }
            else
            {
                result.Errors.AddRange(exception.ValidationErrors.Select(v => new Error {Message = v}));
            }

            return result;
        }

        public override HttpStatusCode StatusCode { get; } = HttpStatusCode.BadRequest;
    }
}