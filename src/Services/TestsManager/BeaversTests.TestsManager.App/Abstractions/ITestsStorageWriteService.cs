using BeaversTests.Common.Binary;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsStorageWriteService
{
    public Task AddTestPackageAsync(
        Guid testPackageId, 
        FileSystemEntity testPackageContent,
        CancellationToken cancellationToken = default);

    Task RemoveTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
}