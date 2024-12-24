namespace BeaversTests.TestRunner;

public abstract class CommandResult
{
    internal byte[] Serialize() => TestRunnerSerialization.Serialize(this);
}