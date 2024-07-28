using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestDrivers;

public class TestDriversResolver(
    IServiceProvider serviceProvider,
    ILogger<TestDriversResolver> logger)
{
    public ITestsExplorer ResolveTestsExplorer(string testType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(testType, nameof(testType));
        logger.LogInformation("Resolving tests explorer for {TestType} testType", testType);
        
        using var scope = serviceProvider.CreateScope();
        return scope.ServiceProvider.GetRequiredKeyedService<ITestsExplorer>(testType);
    }
}