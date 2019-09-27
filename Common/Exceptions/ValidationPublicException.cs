using System;
using System.Collections.Generic;

namespace Common.Exceptions
{
    public class ValidationPublicException : PublicException
    {
        public IEnumerable<string> ValidationErrors { get; }

        public ValidationPublicException(string message = null, Exception innerException = null) : base(message, innerException) { }

        public ValidationPublicException(IEnumerable<string> validationErrors, string message = null, Exception innerException = null) : base(message, innerException)
        {
            ValidationErrors = validationErrors;
        }
    }
}