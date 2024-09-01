using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class DriversController(
    IQueryBus queryBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListAsync(
        GetDriversListQuery.Query queryInput, 
        CancellationToken cancellationToken)
    {
        var result = await queryBus.SendAsync(queryInput, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}