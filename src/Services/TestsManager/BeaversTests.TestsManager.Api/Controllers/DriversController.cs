using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.App;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Dtos.TestDriver;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DriversController(
    ICommandBus commandBus,
    IQueryBus queryBus,
    ILogger<TestsController> logger,
    EntityContentExtractor extractor) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListAsync(
        [FromQuery] GetDriversListQuery.Query queryInput,
        CancellationToken cancellationToken)
    {
        var result = await queryBus.SendAsync(queryInput, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddZipAsync(
        [FromBody] TestDriverZipDto testDriverZipDto,
        CancellationToken cancellationToken)
    {
        return await AddTestDriverAsync<TestDriverZipDto>(testDriverZipDto, cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> AddBase64Async(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private async Task<IActionResult> AddTestDriverAsync<TInput>(
        TestDriverBase testDriverInput,
        CancellationToken cancellationToken)
        where TInput : TestDriverBase
    {
        logger.LogDebug("Adding test driver {Key}", testDriverInput.Key);

        var driverContent = extractor.ExtractContent(testDriverInput);

        var driverContentDto = new NewTestDriverContentDto()
        {
            Directories = driverContent.Directories,
            TestFiles = driverContent.TestFiles
        };

        var newTestDriver = new NewTestDriverDto
        {
            Content = driverContentDto,
            Key = testDriverInput.Key,
            Description = testDriverInput.Description
        };

        var command = new AddTestDriverCommand.Command()
        {
            TestDriver = newTestDriver
        };
        
        var commandResult = await commandBus.SendAsync(command, cancellationToken);

        return Ok(commandResult);
    }
}