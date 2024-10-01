using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestDrivers.Internal;

public class TestDriversResolver(
    TestDriversRegistry registry,
    ILogger<TestDriversResolver> logger)
{
    public ITestsExplorer ResolveTestsExplorer(string testType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(testType, nameof(testType));
        logger.LogInformation("Resolving tests explorer for {TestType} testType", testType);
        
        using var scope = registry.ServiceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredKeyedService<ITestsExplorer>(testType);
    }
}