using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.App;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Dtos.TestPackage;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestsController(
    IQueryBus queryBus,
    ICommandBus commandBus,
    ILogger<TestsController> logger,
    EntityContentExtractor extractor) : ControllerBase
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
    public async Task<IActionResult> AddTestPackageZipAsync(
        TestPackageZipDto testPackageInput,
        CancellationToken cancellationToken)
    {
        return await AddTestPackageAsync(testPackageInput, cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> AddTestPackageBase64Async(
        Base64TestPackageDto testPackageInput,
        CancellationToken cancellationToken)
    {
        return await AddTestPackageAsync(testPackageInput, cancellationToken);
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
    private async Task<IActionResult> AddTestPackageAsync<TInput>(
        TInput testPackageInput,
        CancellationToken cancellationToken)
        where TInput : TestPackageBase
    {
        logger.LogDebug("Adding test package {Name} to Project {Id}", testPackageInput.Name,
            testPackageInput.TestProjectId);

        var testPackageContent = extractor.ExtractContent(testPackageInput);
        
        var testPackageContentDto = new NewTestPackageContentDto()
        {
            Files = testPackageContent.Files,
            Directories = testPackageContent.Directories
        };
        
        var newTestPackage = new NewTestPackageDto()
        {
            Name = testPackageInput.Name,
            Description = testPackageInput.Description,
            TestDriver = testPackageInput.TestPackageType,
            TestProjectId = testPackageInput.TestProjectId,
            Content = testPackageContentDto
        };
        
        var command = new AddTestPackageCommand.Command()
        {
            TestPackage = newTestPackage
        };

        var commandResult = await commandBus.SendAsync(command, cancellationToken);

        return Ok(commandResult);
    }
}