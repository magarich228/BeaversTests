using System;
using System.Net;
using BeaversTests.Platform.Public;

namespace BeaversTests.Client.Http.Exceptions;

public class BeaversTestsClientException : BeaversTestsException
{
    public HttpStatusCode StatusCode { get; }
    public string? ErrorCode { get; }
    public string? ResponseContent { get; }

    public BeaversTestsClientException(string message, HttpStatusCode statusCode, string? errorCode = null, string? responseContent = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ResponseContent = responseContent;
    }

    public BeaversTestsClientException(string message, Exception innerException, HttpStatusCode statusCode)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}