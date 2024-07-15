namespace BeaversTests.TestsManager.App.Exceptions;

public class TestsManagerException(string message) : ApplicationException(message)
{
    public TestsManagerException() : this("Tests manager error.") { }
}