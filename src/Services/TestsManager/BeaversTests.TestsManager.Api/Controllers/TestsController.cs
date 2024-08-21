using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;
using TestPackageDto = BeaversTests.TestsManager.Api.Dtos.TestPackageDto;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestsController(
    IQueryBus queryBus, 
    ICommandBus commandBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjectTestPackagesAsync(
        [FromQuery] GetProjectTestPackagesQuery.Query queryInput,
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.SendAsync(queryInput, cancellationToken);
        
        return Ok(queryResult);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTestPackageInfoAsync(
        [FromQuery] GetTestPackageInfoQuery.Query queryInput,
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.SendAsync(queryInput, cancellationToken);
        
        return Ok(queryResult);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddTestPackageAsync(
        TestPackageDto testPackageInput,
        CancellationToken cancellationToken)
    {
        var (testFiles, directories) = GetTestFilesAndDirectories(testPackageInput);
        
        var command = new AddTestPackageCommand.Command()
        {
            TestPackage = new NewTestPackageDto()
            {
                Name = testPackageInput.Name,
                Description = testPackageInput.Description,
                TestPackageType = testPackageInput.TestPackageType,
                TestProjectId = testPackageInput.TestProjectId,
                TestFiles = testFiles,
                Directories = directories
            }
        };
        
        var commandResult = await commandBus.SendAsync(command, cancellationToken);
        
        return Ok(commandResult);
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveTestPackageAsync(
        [FromQuery] RemoveTestPackageCommand.Command commandInput,
        CancellationToken cancellationToken)
    {
        var commandResult = await commandBus.SendAsync(commandInput, cancellationToken);
        
        return Ok(commandResult);
    }

    [NonAction]
    private (IEnumerable<TestPackageFileInfo> testFiles, IEnumerable<NewTestPackageDirectoryDto> directories) GetTestFilesAndDirectories(
            TestPackageDto testPackageInput)
    {
        List<TestPackageFileInfo> files = new();
        List<NewTestPackageDirectoryDto> directories = new();
        
        files.AddRange(GetTestFiles(testPackageInput.TestFiles));
        
        foreach (var directory in testPackageInput.Directories)
        {
            var newTestPackageDirectory = GetDirectory(directory);
            directories.Add(newTestPackageDirectory);
        }
        
        return (files, directories);
    }
    
    [NonAction]
    private List<TestPackageFileInfo> GetTestFiles(IFormFileCollection testFiles)
    {
        List<TestPackageFileInfo> files = new();
        
        foreach (var file in testFiles)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            
            files.Add(new TestPackageFileInfo
            {
                Name = file.FileName,
                Length = file.Length,
                Content = ms.ToArray(),
                MediaType = file.ContentType
            });
        }
        
        return files;
    }

    [NonAction]
    private NewTestPackageDirectoryDto GetDirectory(TestPackageDirectoryDto testPackageDirectory)
    {
        NewTestPackageDirectoryDto newTestPackageDirectory = new()
        {
            DirectoryName = testPackageDirectory.DirectoryName,
            TestFiles = GetTestFiles(testPackageDirectory.TestFiles),
            Directories = testPackageDirectory.Directories.Select(GetDirectory)
        };

        return newTestPackageDirectory;
    }
}