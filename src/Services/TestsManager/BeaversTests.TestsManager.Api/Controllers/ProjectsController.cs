using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProjectsController(
    ICommandBus commandBus,
    IQueryBus queryBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] GetAllProjectsQuery.Query queryInput, 
        CancellationToken cancellationToken = default)
    {
        var queryResult = await queryBus.SendAsync(queryInput, cancellationToken);

        return Ok(queryResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync(
        [FromQuery] GetProjectByIdQuery.Query queryInput, 
        CancellationToken cancellationToken = default)
    {
        var queryResult = await queryBus.SendAsync(queryInput, cancellationToken);

        return Ok(queryResult);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        CreateProjectCommand.Command commandInput,
        CancellationToken cancellationToken = default)
    {
        var commandResult = await commandBus.SendAsync(commandInput, cancellationToken);

        return Ok(commandResult);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(
        UpdateProjectCommand.Command commandInput,
        CancellationToken cancellationToken = default)
    {
        var commandResult = await commandBus.SendAsync(commandInput, cancellationToken);
        
        return Ok(commandResult);
    }
    
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(
        [FromQuery] RemoveProjectCommand.Command commandInput,
        CancellationToken cancellationToken = default)
    {
        var commandResult = await commandBus.SendAsync(commandInput, cancellationToken);
        
        return Ok(commandResult);
    }
}