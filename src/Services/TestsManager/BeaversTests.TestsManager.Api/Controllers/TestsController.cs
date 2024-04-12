using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using NUnit.Engine;

namespace BeaversTests.TestsManager.Api.Controllers;

[Route("api/[controller]")]
public class TestsController : ControllerBase
{
    private static readonly ConcurrentBag<TestPackage> TestPackages = new();

    [HttpGet("[action]")]
    public async Task<IActionResult> ExploreLoadedTests(CancellationToken ct)
    {
        var sb = new StringBuilder();
        
        foreach (var testPackage in TestPackages)
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
    
    [HttpPost("[action]")]
    public async Task<IActionResult> AddTestAssembly(IFormFile inputTestAssembly, CancellationToken ct)
    {
        await using var asmStream = inputTestAssembly.OpenReadStream();
        
        var assemblyBuffer = new byte[inputTestAssembly.Length];
        var bytesRead = await asmStream.ReadAsync(assemblyBuffer, 0, (int)asmStream.Length, ct);
        
        var asmFileName = inputTestAssembly.FileName;

        if (!System.IO.File.Exists(asmFileName))
        {
            await using var asmFile = System.IO.File.Create(asmFileName);
            await asmFile.WriteAsync(assemblyBuffer, 0, bytesRead, ct);
            await asmFile.FlushAsync(ct);
        }

        var asmFullPath = Path.GetFullPath(asmFileName);

        var inputAsm = Assembly.LoadFrom(asmFullPath);
        var testPackage = new TestPackage(asmFullPath);
        TestPackages.Add(testPackage);
        
        using var engine = TestEngineActivator.CreateInstance();
        using var runner = engine.GetRunner(testPackage);
        var testCount = runner.CountTestCases(TestFilter.Empty);
        
        return Ok($"Test count: {testCount}");
    }

    [HttpPost("[action]")]
    public void AddTestFile(IFormFile testFile)
    {
        
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RunTestAssembly(CancellationToken ct)
    {
        var testPackage = TestPackages.FirstOrDefault();

        if (testPackage is null)
        {
            ModelState.AddModelError(nameof(testPackage), "TestPackages is empty.");
            return BadRequest(ModelState);
        }
        
        using var engine = TestEngineActivator.CreateInstance();
        using var runner = engine.GetRunner(testPackage);
        var testResultXml = runner.Run(null, TestFilter.Empty);
        testResultXml.Normalize();

        return Ok(XElement.Parse(testResultXml.OuterXml).ToString());
    }
}