using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[Route("[controller]/[action]")]
public class ProjectsController : ControllerBase
{
    public ProjectsController()
    {
        
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetById()
    {
        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Accepted();
    }

    [HttpPut]
    public async Task<IActionResult> Update()
    {
        return Accepted();
    }
}