using System;
using System.Net;

namespace BeaversTests.Client.Http.Exceptions;

public class BeaversTestsNetworkException(string message, Exception innerException)
    : BeaversTestsClientException(message, innerException, HttpStatusCode.ServiceUnavailable);