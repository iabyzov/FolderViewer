using System;

namespace Common.Exceptions
{
    public class ObjectNotFoundPublicException : PublicException
    {
        public ObjectNotFoundPublicException(string message = null, Exception innerException = null) : base(message, innerException) {}
    }
}