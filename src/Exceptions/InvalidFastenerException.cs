using System;
using System.Runtime.Serialization;
using NXOpen.Assemblies;

namespace TSG_Library.Exceptions
{
    [Serializable]
    internal class InvalidFastenerException : Exception
    {
        public InvalidFastenerException()
        {
        }

        public InvalidFastenerException(string message) : base(message)
        {
        }

        public InvalidFastenerException(Component component)
        {
        }

        public InvalidFastenerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidFastenerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}