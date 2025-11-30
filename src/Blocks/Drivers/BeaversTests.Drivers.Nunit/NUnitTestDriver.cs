using System.Reflection;
using BeaversTests.Drivers.Abstractions;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestSuite = BeaversTests.Drivers.Abstractions.TestSuite;

namespace BeaversTests.Drivers.Nunit;

public class NUnitTestDriver : ITestDriver
{
    private readonly ITestAssemblyRunner _runner;
    private readonly NUnitTestListener _nunitListener;
    private readonly TestSuite _loadedSuites;

    public NUnitTestDriver()
    {
        _runner = new NUnitTestAssemblyRunner(
            new DefaultTestAssemblyBuilder());
        _nunitListener = new NUnitTestListener();
        // TODO: Привести к единому виду на всех драйверах
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
            LoadTestInternal(nunitTest, suite);
        }
        
        _loadedSuites.TestSuites.Add(suite);
    }

    public TestSuite Explore()
    {
        return _loadedSuites;
    }

    public Task RunAsync(Abstractions.ITestListener listener, RunStrategy strategy)
    {
        _nunitListener.NUnitRunnerEvent += LocalEventHandler;

        try
        {
            _runner.Run(_nunitListener, TestFilter.Empty);
        }
        finally
        {
            _nunitListener.NUnitRunnerEvent -= LocalEventHandler;
        }
        
        return Task.CompletedTask;

        async void LocalEventHandler(object? sender, TestEvent e) => await NUnitListenerEventHandler(sender, e, listener);
    }

    private void LoadTestInternal(ITest nunitTest, TestSuite suite)
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
            throw new NunitDriverException($"Unknown Nunit test type: {nunitTest.TestType}");
        }

        foreach (var nunitSubTest in nunitTest.Tests)
        {
            LoadTestInternal(nunitSubTest, suite);
        }
    }
    
    private async Task NUnitListenerEventHandler(object? sender, TestEvent e, Abstractions.ITestListener listener) =>
        await listener.SendAsync(e);
}