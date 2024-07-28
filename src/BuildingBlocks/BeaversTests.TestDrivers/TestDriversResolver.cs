using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestDrivers;

public class TestDriversResolver(IServiceProvider serviceProvider)
{
    public ITestsExplorer ResolveTestsExplorer(string testType)
    {
        return serviceProvider.GetRequiredKeyedService<ITestsExplorer>(testType);
    }
}