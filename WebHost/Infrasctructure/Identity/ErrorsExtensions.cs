using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebHost.Infrasctructure.Identity
{
    internal static class ErrorsExtensions
    {
        public static void AddErrors(this ModelStateDictionary modelState, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }
        }
    }
}