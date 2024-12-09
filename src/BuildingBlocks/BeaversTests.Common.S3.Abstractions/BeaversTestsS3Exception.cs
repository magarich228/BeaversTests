namespace BeaversTests.Common.S3.Abstractions;

public class BeaversTestsS3Exception(string message, Exception? innerException = null) 
    : Exception(message, innerException) { }