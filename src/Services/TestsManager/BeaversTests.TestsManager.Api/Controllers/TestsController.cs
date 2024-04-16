using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Engine;

namespace BeaversTests.TestsManager.Api.Controllers;

[Route("api/[controller]/[action]")]
public class TestsController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, TestPackage> TestPackages = new();

    [HttpGet]
    public async Task<IActionResult> ExploreLoadedTests(CancellationToken ct)
    {
        var sb = new StringBuilder();
        
        foreach (var testPackage in TestPackages.Values)
        {
            await Task.Run(() =>
            {
                var exploreXml = TestEngineActivator.CreateInstance()
                    .GetRunner(testPackage)
                    .Explore(TestFilter.Empty);

                exploreXml.Normalize();

                sb.Append(XElement.Parse(exploreXml.OuterXml, LoadOptions.PreserveWhitespace));
            }, ct);
        }
        
        return Ok(sb.ToString());
    }
    
    [HttpPost]
    public async Task<IActionResult> AddTestAssembly(IFormFile inputTestAssembly, CancellationToken ct)
    {
        await using var asmStream = inputTestAssembly.OpenReadStream();
        
        var assemblyBuffer = new byte[inputTestAssembly.Length];
        var bytesRead = await asmStream.ReadAsync(assemblyBuffer, 0, (int)asmStream.Length, ct);
        
        var asmFileName = inputTestAssembly.FileName;

        await using (var asmFile = System.IO.File.Create(asmFileName))
        {
            await asmFile.WriteAsync(assemblyBuffer, 0, bytesRead, ct);
            await asmFile.FlushAsync(ct);
        }

        var asmFullPath = Path.GetFullPath(asmFileName);

        var inputAsm = Assembly.LoadFrom(asmFullPath);
        var testPackage = new TestPackage(asmFullPath);
        TestPackages.AddOrUpdate(asmFileName, testPackage, (name, package) =>
        {
            if (TestPackages.TryGetValue(name, out var existTestPackage))
            {
                TestPackages.Remove(name, out existTestPackage);
            }

            if (!TestPackages.TryAdd(name, package))
            {
                throw new ApplicationException($"Test package with name: {name} has not been added.");
            }

            return package;
        });
        
        using var engine = TestEngineActivator.CreateInstance();
        using var runner = engine.GetRunner(testPackage);
        var testCount = runner.CountTestCases(TestFilter.Empty);
        
        return Ok($"Test count: {testCount}");
    }

    [HttpPost]
    public void AddTestFile(IFormFile testFile)
    {
        
    }

    [HttpPost]
    public IActionResult RunTestAssembly(string testAssemblyName, CancellationToken ct)
    {
        if (!TestPackages.TryGetValue(testAssemblyName, out var testPackage))
        {
            ModelState.AddModelError(nameof(testPackage), "TestPackage not found.");
            return BadRequest(ModelState);
        }
        
        using var engine = TestEngineActivator.CreateInstance();
        using var runner = engine.GetRunner(testPackage);
        var testResultXml = runner.Run(null, TestFilter.Empty);
        testResultXml.Normalize();

        return Ok(XElement.Parse(testResultXml.OuterXml).ToString());
    }
}