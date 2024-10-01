using System.Reflection;
using BeaversTests.TestDrivers;
using Microsoft.Extensions.Logging;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace BeaversTests.NUnit.Driver;

public class NUnitTestsExplorer() : ITestsExplorer<NUnitDriverKey>
{
    private readonly ILogger<NUnitTestsExplorer> _logger = new Logger<NUnitTestsExplorer>(new LoggerFactory());
    
    public DriverKey<NUnitDriverKey> DriverKey { get; } = new();

    public IEnumerable<TestSuite> GetTestSuites(Assembly testsAssembly)
    {
        ArgumentNullException.ThrowIfNull(testsAssembly);
        
        _logger.LogDebug("Exploring assembly location: {AssemblyLocation}", testsAssembly.Location);
        _logger.LogDebug("Assembly exists: {AssemblyExists}", File.Exists(testsAssembly.Location));
        
        var nunitRunner = new NUnitTestAssemblyRunner(
            new DefaultTestAssemblyBuilder());

        var loadedTest = nunitRunner.Load(testsAssembly, new Dictionary<string, object>());
        _logger.LogDebug("Loaded {TestsCount} tests by nunit framework.", loadedTest.TestCaseCount);

        var loadedTests = loadedTest.ToFullList();
        _logger.LogDebug("Loaded {TestsCount} tests list by explorer.", loadedTests.Count);
        
        var testSuites = GetTestSuitesInternal(loadedTests);
        
        var result = testSuites.Select(s => new TestSuite
        {
            Id = s.Id,
            Name = s.Name,
            Tests = GetTestCasesInternal(s)
                .Select(t => new Test
            {
                Name = t.Name
            })
        });

        return result;
    }

    public IEnumerable<TestSuite> GetTestSuites(string testAssemblyPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(testAssemblyPath);
        
        var testAssembly = Assembly.LoadFrom(testAssemblyPath);

        return GetTestSuites(testAssembly);
    }

    private IEnumerable<ITest> GetTestSuitesInternal(IEnumerable<ITest> tests)
    {
        return tests.Where(TestsExtensions.IsTestFixture);
    }
    
    private IEnumerable<ITest> GetTestCasesInternal(ITest testSuite)
    {
        return testSuite
            .ToChildList()
            .Where(TestsExtensions.IsTestCase);
    }
}