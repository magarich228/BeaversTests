namespace BeaversTests.TestRunnerAgent.App.Abstractions;

public interface ITestsStorageReadService
{
    public Task<IDictionary<string, byte[]>> GetTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
}