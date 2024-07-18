using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestsController(
    IQueryBus queryBus, 
    ICommandBus commandBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjectTestPackages(
        [FromQuery] GetProjectTestPackagesQuery.Query queryInput,
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.Send(queryInput, cancellationToken);
        
        return Ok(queryResult);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddTestPackage(
        TestPackageDto testPackageInput,
        CancellationToken cancellationToken)
    {
        var testPackageAssemblies = testPackageInput.TestAssemblies
            .Select(a =>
            {
                using var ms = new MemoryStream();
                a.CopyTo(ms);
                return ms.ToArray();
            });
        
        // TODO: map dto to command
        var command = new AddTestPackageCommand.Command
        {
            Name = testPackageInput.Name,
            TestAssemblies = testPackageAssemblies,
            ItemPaths = testPackageInput.ItemPaths,
            TestProjectId = testPackageInput.TestProjectId,
            TestPackageType = testPackageInput.TestPackageType,
            Description = testPackageInput.Description
        };
        
        var commandResult = await commandBus.Send(command, cancellationToken);
        
        return Ok(commandResult);
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveTestPackage(
        [FromQuery] RemoveTestPackageCommand.Command commandInput,
        CancellationToken cancellationToken)
    {
        var commandResult = await commandBus.Send(commandInput, cancellationToken);
        
        return Ok(commandResult);
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
}