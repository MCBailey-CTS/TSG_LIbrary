using System;
using System.Runtime.Serialization;

namespace TSG_Library.Exceptions
{
    [Serializable]
    internal class NoDynamicBlockException : Exception
    {
        public NoDynamicBlockException()
        {
        }

        public NoDynamicBlockException(string message) : base(message)
        {
        }

        public NoDynamicBlockException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoDynamicBlockException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}