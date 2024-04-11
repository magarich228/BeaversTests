using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using NUnit.Engine;
using TestFilter = NUnit.Engine.TestFilter;

namespace BeaversTests.TestsManager.Api.Controllers;

[Route("api/[controller]")]
public class TestsController : ControllerBase
{
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
        
        var engine = TestEngineActivator.CreateInstance();
        var runner = engine.GetRunner(testPackage);
        var testCount = runner.CountTestCases(TestFilter.Empty);
        
        return Ok($"Test count: {testCount}");
    }

    [HttpPost("[action]")]
    public void AddTestFile(IFormFile testFile)
    {
        
    }
}