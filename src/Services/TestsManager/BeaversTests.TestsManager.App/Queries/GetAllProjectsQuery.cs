using AutoMapper;
using BeaversTests.Common.Application;
using BeaversTests.Common.Application.Models;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Dtos.TestProject;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.Queries;

public abstract class GetAllProjectsQuery
{
    public class Query : PaginatedQuery, IQuery<Result>;
    
    public class Result
    {
        public IEnumerable<TestProjectDto> TestProjects { get; init; } = null!;
    }
    
    public class Validator : AbstractValidator<Query>
    {
        public Validator() { }
    }
    
    public class Handler(
        ITestsManagerContext db,
        IUserService userService,
        IMapper mapper,
        ILogger<Handler> logger) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query query, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("Get all test projects query handler called.");
            
            // TODO: add order
            var testProjects = db.TestProjects
                .Where(t => t.UserCreatorId == userService.GetCurrentUserId())
                .PageBy(query)
                .AsNoTracking();
            
            return new Result
            {
                TestProjects = await mapper
                    .ProjectTo<TestProjectDto>(testProjects)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}