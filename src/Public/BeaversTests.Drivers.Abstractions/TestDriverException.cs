using System;
using System.Runtime.Serialization;
using BeaversTests.Platform.Public;

namespace BeaversTests.Drivers.Abstractions
{
    public class TestDriverException : BeaversTestsException
    {
        public TestDriverException(string message) : base(message) { }
        public TestDriverException(string message, Exception innerException) : base(message, innerException) { }
        public TestDriverException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
    }
}