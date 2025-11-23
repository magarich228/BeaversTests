using System;
using System.Runtime.Serialization;

namespace BeaversTests.Platform.Public
{
    public class BeaversTestsException : Exception
    {
        public BeaversTestsException(string message) : base(message) { }
        public BeaversTestsException(string message, Exception innerException) : 
            base(message, innerException) { }
        
        public BeaversTestsException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
    }
}