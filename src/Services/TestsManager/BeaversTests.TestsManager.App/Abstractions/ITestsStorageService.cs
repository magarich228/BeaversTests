namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsStorageService
{
    public Task<IDictionary<string, byte[]>> GetTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
    
    public Task AddTestPackageAsync(
        Guid testPackageId, 
        IEnumerable<byte[]> testAssemblies, 
        IEnumerable<string> assemblyPaths, 
        CancellationToken cancellationToken = default);

    Task RemoveTestPackageAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
}