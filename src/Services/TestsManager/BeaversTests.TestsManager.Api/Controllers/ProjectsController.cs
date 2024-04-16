using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[Route("[controller]/[action]")]
public class ProjectsController : ControllerBase
{
    public ProjectsController()
    {
        
    }

    public async Task<IActionResult> GetAll()
    {
        return Empty;
    }

    public async Task<IActionResult> GetById()
    {
        return NotFound();
    }

    public async Task<IActionResult> Create()
    {
        return Accepted();
    }

    public async Task<IActionResult> Update()
    {
        return Accepted();
    }
}