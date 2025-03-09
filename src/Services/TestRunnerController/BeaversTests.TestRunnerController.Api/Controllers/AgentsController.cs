using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestRunnerController.App.Commands;
using BeaversTests.TestRunnerController.App.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestRunnerController.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]/[action]")]
public class AgentsController(
    IQueryBus queryBus,
    ICommandBus commandBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAgentsAsync(CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.SendAsync(new GetAgentsQuery.Query(), cancellationToken);
        
        return Ok(queryResult);
    }

    [HttpGet]
    public async Task<IActionResult> GetAgentConnectionKeysAsync(CancellationToken cancellationToken)
    {
        var query = new GetAgentConnectionKeysQuery.Query();

        var queryResult = await queryBus.SendAsync(query, cancellationToken);
        
        return Ok(queryResult);
    }
    
    [HttpPost]
    public async Task<IActionResult> GenerateAgentConnectionKeyAsync(CancellationToken cancellationToken)
    {
        var command = new CreateUserControllerKeyCommand.Command();

        var commandResult = await commandBus.SendAsync(command, cancellationToken);

        return Ok(commandResult);
    }
}