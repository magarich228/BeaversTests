using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestRunnerController.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestRunnerController.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AgentsController(
    IQueryBus queryBus,
    ILogger<AgentsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAgentsAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting agents...");

        var queryResult = await queryBus.SendAsync(new GetAgentsQuery.Query(), cancellationToken);
        
        return Ok(queryResult);
    }
}