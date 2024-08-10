namespace BeaversTests.Client;

// TODO: move to abstractions
public interface ITestsManagerClient
{
    Task AddTestPackageAsync(
        Guid testPackageId,
        IEnumerable<byte[]> testAssemblies,
        IEnumerable<string> assemblyPaths,
        CancellationToken cancellationToken = default);
}

// public class TestsManagerClient : ServiceClientBase, ITestsManagerClient
// {
//     
// }