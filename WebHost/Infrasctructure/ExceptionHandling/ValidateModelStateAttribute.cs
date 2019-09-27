using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using WebHost.ClientApi.Errors;

namespace WebHost.Infrasctructure.ExceptionHandling
{
    public class ValidateModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var validationErrors = context.ModelState
                .Where(e => e.Value.Errors.Any())
                .Select(e => GetErrorFromModelStateErrorCollection(e.Value.Errors))
                .ToArray();

            if (validationErrors.Any())
            {
                context.Result = CreateValidationErrorDetails(validationErrors);
            }
        }

        private static ActionResult CreateValidationErrorDetails(params Error[] errors)
        {
            return new BadRequestObjectResult(new ApiError { Errors = errors.ToList() });
        }

        private static Error GetErrorFromModelStateErrorCollection(ModelErrorCollection errors)
        {
            var errorMessages = errors.Select(e => e.ErrorMessage);
            return new Error(string.Join(Environment.NewLine, errorMessages));
        }
    }
}