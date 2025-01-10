namespace BeaversTests.TestRunner;

public abstract class CommandResult
{
    internal Stream Serialize() => TestRunnerSerialization.Serialize(this);
}