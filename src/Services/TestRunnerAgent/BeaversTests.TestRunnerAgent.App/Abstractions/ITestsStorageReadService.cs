using BeaversTests.Common.Binary;

namespace BeaversTests.TestRunnerAgent.App.Abstractions;

public interface ITestsStorageReadService
{
    public Task<TestPackageContent> GetTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
}