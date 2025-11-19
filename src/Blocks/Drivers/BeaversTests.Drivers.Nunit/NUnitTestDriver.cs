using System.Reflection;
using BeaversTests.Drivers.Abstractions;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using ITestListener = BeaversTests.Drivers.Abstractions.ITestListener;
using TestSuite = BeaversTests.Drivers.Abstractions.TestSuite;

namespace BeaversTests.Drivers.Nunit;

public class NUnitTestDriver : ITestDriver
{
    private readonly ITestAssemblyRunner _runner;
    private readonly TestSuite _loadedSuites;

    public NUnitTestDriver()
    {
        _runner = new NUnitTestAssemblyRunner(
            new DefaultTestAssemblyBuilder());
        _loadedSuites = new TestSuite()
        {
            Id = "global",
            Name = "global",
            Description = "All loaded tests"
        };
    }
    
    public void Load(string path)
    {
        var assembly = Assembly.LoadFrom(path);
        var nunitTests = _runner.Load(assembly, new Dictionary<string, object>()); // TODO: загрузка нескольких сборок?

        var suite = nunitTests.ToTestSuite();

        if (_loadedSuites.TestSuites.Any(s => s.Id == suite.Id))
        {
            return;
        }
        
        foreach (var nunitTest in nunitTests.Tests)
        {
            ProcessTest(nunitTest, suite);
        }
        
        _loadedSuites.TestSuites.Add(suite);
    }

    public TestSuite Explore()
    {
        return _loadedSuites;
    }

    public async Task Run(ITestListener listener)
    {
        throw new NotImplementedException();
    }

    private void ProcessTest(ITest nunitTest, TestSuite suite)
    {
        if (nunitTest.IsTestCase())
        {
            var testCase = nunitTest.ToTestCase();
            suite.Tests.Add(testCase);
        }
        else if (nunitTest.IsTestSuite() || nunitTest.IsTestFixture())
        {
            var newSuite = nunitTest.ToTestSuite();
            suite.TestSuites.Add(newSuite);
            suite = newSuite;
        }
        else
        {
            throw new Exception($"Unknown Nunit test type: {nunitTest.TestType}");
        }

        foreach (var nunitSubTest in nunitTest.Tests)
        {
            ProcessTest(nunitSubTest, suite);
        }
    }
}