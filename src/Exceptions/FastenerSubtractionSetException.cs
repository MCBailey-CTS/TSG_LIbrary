using System;
using System.Runtime.Serialization;

namespace TSG_Library.Exceptions
{
    [Serializable]
    internal class FastenerSubtractionSetException : Exception
    {
        public FastenerSubtractionSetException(string fastenerDelimeter)
        {
        }

        //public FastenerSubtractionSetException(string message) : base(message)
        //{
        //}

        public FastenerSubtractionSetException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FastenerSubtractionSetException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}