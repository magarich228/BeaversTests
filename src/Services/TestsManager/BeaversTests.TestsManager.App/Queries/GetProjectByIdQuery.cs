using AutoMapper;
using BeaversTests.Common.Application;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Dtos.TestProject;
using BeaversTests.TestsManager.Core.TestProject;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.Queries;

public abstract class GetProjectByIdQuery
{
    public class Query : IQuery<Result>
    {
        public required Guid ProjectId { get; init; }
    }

    public class Result
    {
        public TestProjectDto? TestProject { get; init; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITestsManagerContext db,
            IUserService userService)
        {
            RuleFor(q => q.ProjectId)
                .NotEmpty()
                .MustAsync((id, token) => db.TestProjects
                    .Where(t => t.UserCreatorId == userService.GetCurrentUserId())
                    .AnyAsync(x => x.Id == id, token))
                .WithMessage("Project with this id does not exist");
        }
    }

    public class Handler(
        ITestsManagerContext db,
        IUserService userService,
        IMapper mapper,
        ILogger<Handler> logger) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query query, CancellationToken cancellationToken = default)
        {
            logger.LogDebug($"Get test project by id ({query.ProjectId}) query handler called.");
            
            var testProject = await db.TestProjects
                .Where(t => t.UserCreatorId == userService.GetCurrentUserId())
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == query.ProjectId, cancellationToken);

            if (testProject is null)
            {
                return new Result();
            }
            
            return new Result
            {
                TestProject = mapper.Map<TestProject, TestProjectDto>(testProject)
            };
        }
    }
}