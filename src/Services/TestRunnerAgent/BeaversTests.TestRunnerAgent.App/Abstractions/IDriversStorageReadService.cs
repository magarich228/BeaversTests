using BeaversTests.Common.Binary;

namespace BeaversTests.TestRunnerAgent.App.Abstractions;

public interface IDriversStorageReadService
{
    public Task<TestDriverContent> GetTestDriverAsync(
        string testDriverKey,
        CancellationToken cancellationToken = default);
}