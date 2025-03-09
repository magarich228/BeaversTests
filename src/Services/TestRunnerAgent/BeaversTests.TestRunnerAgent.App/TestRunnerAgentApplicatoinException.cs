namespace BeaversTests.TestRunnerAgent.App;

public class TestRunnerAgentException(
    string? message, 
    Exception? innerException = null) : 
    ApplicationException(message, innerException) { }