using System;

namespace Common.Exceptions
{
    public class PublicException : Exception
    {
        public PublicException()
        {
        }

        public PublicException(string message) : base(message)
        {
        }

        public PublicException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}