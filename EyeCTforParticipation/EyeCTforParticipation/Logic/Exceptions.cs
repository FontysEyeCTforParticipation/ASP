using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace EyeCTforParticipation.Logic
{
    [Serializable]
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException()
        {
        }

        public InvalidTokenException(string message) : base(message)
        {
        }

        public InvalidTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidTokenException(SerializationInfo info, StreamingContext context): base(info, context)
        {
        }
    }
}