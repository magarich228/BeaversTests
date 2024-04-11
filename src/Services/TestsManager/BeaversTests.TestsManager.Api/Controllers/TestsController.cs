using System.Collections.Concurrent;
using System.Reflection;
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
        var exploreNodes = await Task.Run(() => 
            TestPackages.Select(p =>
            XElement.Parse(TestEngineActivator.CreateInstance()
                .GetRunner(p)
                .Explore(TestFilter.Empty)
                .OuterXml, LoadOptions.PreserveWhitespace).ToString()), ct);
        
        return Ok(exploreNodes);
    }
    
    [HttpPost("[action]")]
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
}