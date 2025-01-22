namespace BeaversTests.TestRunnerController.Core;

public interface ITestAgentsPool
{
    Task<Guid?> GetAsync(string userId, CancellationToken cancellationToken = default);
}