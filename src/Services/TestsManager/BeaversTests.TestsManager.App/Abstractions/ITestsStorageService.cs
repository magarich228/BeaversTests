using BeaversTests.TestsManager.Core.TestPackage;

namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsStorageService
{
    public Task<IDictionary<string, byte[]>> GetTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
    
    public Task AddTestPackageAsync(
        Guid testPackageId, 
        TestPackageContent testPackageContent,
        CancellationToken cancellationToken = default);

    Task RemoveTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
}