using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.App.Commands;
using BeaversTests.TestsManager.App.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BeaversTests.TestsManager.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TestsController(
    IQueryBus queryBus, 
    ICommandBus commandBus) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProjectTestPackages(
        [FromQuery] GetProjectTestPackagesQuery.Query queryInput,
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.SendAsync(queryInput, cancellationToken);
        
        return Ok(queryResult);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTestPackageInfo(
        [FromQuery] GetTestPackageInfoQuery.Query queryInput,
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.SendAsync(queryInput, cancellationToken);
        
        return Ok(queryResult);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddTestPackage(
        TestPackageDto testPackageInput,
        CancellationToken cancellationToken)
    {
        // Переделать на DTO с вложенными объектами поддиректорий (имя директории, IFormFileCollection)
        var testPackageAssemblies = testPackageInput.TestAssemblies
            .Select(a =>
            {
                using var ms = new MemoryStream();
                a.CopyTo(ms);
                return ms.ToArray();
            });
        
        // TODO: map dto to command
        var command = new AddTestPackageCommand.Command
        {
            Name = testPackageInput.Name,
            TestAssemblies = testPackageAssemblies,
            ItemPaths = testPackageInput.ItemPaths,
            TestProjectId = testPackageInput.TestProjectId,
            TestPackageType = testPackageInput.TestPackageType,
            Description = testPackageInput.Description
        };
        
        var commandResult = await commandBus.SendAsync(command, cancellationToken);
        
        return Ok(commandResult);
    }
    
    [HttpDelete]
    public async Task<IActionResult> RemoveTestPackage(
        [FromQuery] RemoveTestPackageCommand.Command commandInput,
        CancellationToken cancellationToken)
    {
        var commandResult = await commandBus.SendAsync(commandInput, cancellationToken);
        
        return Ok(commandResult);
    }
}