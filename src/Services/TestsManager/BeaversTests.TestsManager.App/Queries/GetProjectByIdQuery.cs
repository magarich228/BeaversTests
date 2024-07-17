using AutoMapper;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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
        public Validator(ITestsManagerContext db)
        {
            RuleFor(q => q.ProjectId)
                .NotEmpty()
                .MustAsync((id, token) => db.TestProjects
                    .AnyAsync(x => x.Id == id, token))
                .WithMessage("Project with this id does not exist");
        }
    }

    public class Handler(
        ITestsManagerContext db,
        IMapper mapper) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query query, CancellationToken cancellationToken = default)
        {
            var testProject = await db.TestProjects
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