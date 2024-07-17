using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ProjectsController(
    ICommandBus commandBus,
    IQueryBus queryBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetAllProjectsQuery.Query queryInput, 
        CancellationToken cancellationToken = default)
    {
        var queryResult = await queryBus.Send(queryInput, cancellationToken);

        return Ok(queryResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(
        [FromQuery] GetProjectByIdQuery.Query queryInput, 
        CancellationToken cancellationToken = default)
    {
        var queryResult = await queryBus.Send(queryInput, cancellationToken);

        return Ok(queryResult);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProjectCommand.Command createProjectInput,
        CancellationToken cancellationToken = default)
    {
        var commandResult = await commandBus.Send(createProjectInput, cancellationToken);

        return Ok(commandResult);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateProjectCommand.Command updateProjectInput,
        CancellationToken cancellationToken = default)
    {
        var commandResult = await commandBus.Send(updateProjectInput, cancellationToken);
        
        return Ok(commandResult);
    }
}