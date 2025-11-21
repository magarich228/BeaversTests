using System.Runtime.Serialization;
using BeaversTests.Drivers.Abstractions;

namespace BeaversTests.Drivers.Nunit;

public class NunitDriverException : TestDriverException
{
    public NunitDriverException(string message) : base(message) { }
    public NunitDriverException(string message, Exception innerException) : base(message, innerException) { }
    public NunitDriverException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context) { }
}