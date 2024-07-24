namespace BeaversTests.TestsManager.App.Abstractions;

public interface ITestsStorageService
{
    public Task AddTestAssemblyAsync(
        Guid testPackageId, 
        IEnumerable<byte[]> testAssemblies, 
        IEnumerable<string> assemblyPaths, 
        CancellationToken cancellationToken = default);

    Task RemoveTestAssemblyAsync(
        Guid testPackageId,
        CancellationToken cancellationToken = default);
}