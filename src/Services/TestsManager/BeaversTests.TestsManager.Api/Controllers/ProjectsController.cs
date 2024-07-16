﻿using BeaversTests.Common.CQRS.Commands;
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
        GetAllProjectsQuery.Query queryInput, 
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.Send(queryInput, cancellationToken);

        return Ok(queryResult.TestProjects);
    }

    [HttpGet]
    public async Task<IActionResult> GetById(
        GetProjectByIdQuery.Query queryInput, 
        CancellationToken cancellationToken)
    {
        var queryResult = await queryBus.Send(queryInput, cancellationToken);

        return Ok(queryResult.TestProject);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProjectCommand.Command createProjectInput,
        CancellationToken cancellationToken)
    {
        var commandResult = await commandBus.Send(createProjectInput, cancellationToken);

        return Ok(commandResult.TestProjectId);
    }

    [HttpPut]
    public async Task<IActionResult> Update(
        UpdateProjectCommand.Command updateProjectInput,
        CancellationToken cancellationToken)
    {
        var commandResult = await commandBus.Send(updateProjectInput, cancellationToken);
        
        return Ok(commandResult.TestProject);
    }
}