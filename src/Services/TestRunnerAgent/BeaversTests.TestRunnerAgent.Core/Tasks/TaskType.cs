namespace BeaversTests.TestRunnerAgent.Core.Tasks;

public enum TaskType : int
{
    Unknown = 0,
    DriverValidation = 1,
    TestPackageValidation = 2,
    TestRun = 3
}