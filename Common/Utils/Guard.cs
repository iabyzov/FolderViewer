using System;

namespace Common.Utils
{
    public static class Guard
    {
        public static void IsNotNull(object value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}