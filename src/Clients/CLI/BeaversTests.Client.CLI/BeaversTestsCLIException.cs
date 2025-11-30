using System.Runtime.Serialization;
using BeaversTests.Platform.Public;

namespace BeaversTests.Client.CLI;

public class BeaversTestsCliException : BeaversTestsException
{
    public BeaversTestsCliException(string message) : base(message) { }
    public BeaversTestsCliException(string message, Exception innerException) : base(message, innerException) { }
    public BeaversTestsCliException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
}