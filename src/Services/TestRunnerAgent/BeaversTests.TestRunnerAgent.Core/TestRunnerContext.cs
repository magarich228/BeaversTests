namespace BeaversTests.TestRunnerAgent.Core;

public class TestRunnerContext(string controllerConnectionKey)
{
    public Guid Id { get; } = Guid.NewGuid();

    public string ControllerConnectionKey { get; } = controllerConnectionKey;
}