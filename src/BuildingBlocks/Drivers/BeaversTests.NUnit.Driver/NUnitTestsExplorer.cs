using System.Reflection;
using BeaversTests.TestDrivers;
using BeaversTests.TestDrivers.Models;
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

// private static readonly ConcurrentDictionary<string, TestPackages> TestPackages = new();
    //
    // [HttpGet]
    // public async Task<IActionResult> ExploreLoadedTests(CancellationToken cancellationToken)
    // {
    //     var sb = new StringBuilder();
    //     
    //     foreach (var testPackage in TestPackages.Values)
    //     {
    //         await Task.Run(() =>
    //         {
    //             var exploreXml = TestEngineActivator.CreateInstance()
    //                 .GetRunner(testPackage)
    //                 .Explore(TestFilter.Empty);
    //
    //             exploreXml.Normalize();
    //
    //             sb.Append(XElement.Parse(exploreXml.OuterXml, LoadOptions.PreserveWhitespace));
    //         }, cancellationToken);
    //     }
    //     
    //     return Ok(sb.ToString());
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> AddTestAssemblyAsync(IFormFile inputTestAssembly, CancellationToken cancellationToken)
    // {
    //     await using var asmStream = inputTestAssembly.OpenReadStream();
    //     
    //     var assemblyBuffer = new byte[inputTestAssembly.Length];
    //     var bytesRead = await asmStream.ReadAsync(assemblyBuffer, 0, (int)asmStream.Length, cancellationToken);
    //     
    //     var asmFileName = inputTestAssembly.FileName;
    //
    //     await using (var asmFile = System.IO.File.Create(asmFileName))
    //     {
    //         await asmFile.WriteAsync(assemblyBuffer, 0, bytesRead, cancellationToken);
    //         await asmFile.FlushAsync(cancellationToken);
    //     }
    //
    //     var asmFullPath = Path.GetFullPath(asmFileName);
    //
    //     var inputAsm = Assembly.LoadFrom(asmFullPath);
    //     var testPackage = new TestPackages(asmFullPath);
    //     TestPackages.AddOrUpdate(asmFileName, testPackage, (name, package) =>
    //     {
    //         if (TestPackages.TryGetValue(name, out var existTestPackage))
    //         {
    //             TestPackages.Remove(name, out existTestPackage);
    //         }
    //
    //         if (!TestPackages.TryAdd(name, package))
    //         {
    //             throw new ApplicationException($"Test package with name: {name} has not been added.");
    //         }
    //
    //         return package;
    //     });
    //     
    //     using var engine = TestEngineActivator.CreateInstance();
    //     using var runner = engine.GetRunner(testPackage);
    //     var testCount = runner.CountTestCases(TestFilter.Empty);
    //     
    //     return Ok($"Test count: {testCount}");
    // }
    //
    // [HttpPost]
    // public IActionResult RunTestAssembly(string testAssemblyName, CancellationToken cancellationToken)
    // {
    //     if (!TestPackages.TryGetValue(testAssemblyName, out var testPackage))
    //     {
    //         ModelState.AddModelError(nameof(testPackage), "TestPackages not found.");
    //         return BadRequest(ModelState);
    //     }
    //     
    //     using var engine = TestEngineActivator.CreateInstance();
    //     using var runner = engine.GetRunner(testPackage);
    //     var testResultXml = runner.Run(null, TestFilter.Empty);
    //     testResultXml.Normalize();
    //
    //     return Ok(XElement.Parse(testResultXml.OuterXml).ToString());
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> OneTimeRun(IFormFile assemblyFile, CancellationToken cancellationToken)
    // {
    //     await using var read = assemblyFile.OpenReadStream();
    //     var fileLength = assemblyFile.Length;
    //
    //     var assemblyBuffer = new byte[fileLength];
    //     
    //     var bytesRead = await read.ReadAsync(assemblyBuffer, 0, (int)fileLength, cancellationToken);
    //
    //     if (bytesRead < fileLength)
    //     {
    //         return BadRequest();
    //     }
    //     
    //     var testAssembly = Assembly.Load(assemblyBuffer);
    //     var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
    //
    //     var tests = runner.Load(testAssembly, new Dictionary<string, object>());
    //     
    //     return Ok(tests.TestCaseCount);
    // }