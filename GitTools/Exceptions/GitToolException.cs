using System;
using System.Runtime.Serialization;

namespace GitTools.Exceptions
{
    public class GitToolException : Exception
    {
        public GitToolException()
        {
        }

        public GitToolException(string message) : base(message)
        {
        }

        public GitToolException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected GitToolException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
