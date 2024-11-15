namespace BeaversTests.TestRunnerController.Core;

public interface ITestAgentsPool
{
    Task<Guid?> GetAsync(CancellationToken cancellationToken = default);
}