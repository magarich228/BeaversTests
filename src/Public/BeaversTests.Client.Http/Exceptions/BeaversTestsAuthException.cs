using System.Net;

namespace BeaversTests.Client.Http.Exceptions;

public class BeaversTestsAuthException(string message, HttpStatusCode statusCode = HttpStatusCode.Unauthorized)
    : BeaversTestsClientException(message, statusCode);