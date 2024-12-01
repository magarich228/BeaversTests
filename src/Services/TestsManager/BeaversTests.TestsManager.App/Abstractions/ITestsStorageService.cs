using BeaversTests.TestsManager.Core;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsStorageService
{
    public Task AddTestPackageAsync(
        Guid testPackageId, 
        FileSystemEntity testPackageContent,
        CancellationToken cancellationToken = default);

    Task RemoveTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
}