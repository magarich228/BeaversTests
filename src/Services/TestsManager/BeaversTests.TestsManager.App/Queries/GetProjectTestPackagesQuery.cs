using AutoMapper;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Queries;

public abstract class GetProjectTestPackagesQuery
{
    public class Query : IQuery<Result>
    {
        public required Guid TestProjectId { get; init; }
    }
    
    public class Result
    {
        public required IEnumerable<TestPackageDto> TestPackages { get; init; }
    }
    
    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITestsManagerContext db)
        {
            RuleFor(q => q.TestProjectId)
                .NotEmpty()
                .MustAsync(async (q, id, token) => 
                    await db.TestProjects.AnyAsync(t => t.Id == id, token))
                .WithMessage("Test project not found.");
        }
    }
    
    public class Handler(
        ITestsManagerContext db,
        IMapper mapper) : IQueryHandler<Query, Result>
    {
        public Task<Result> Handle(Query query, CancellationToken cancellationToken)
        {
            var testPackages = db.TestProjects
                .Include(t => t.TestPackages)
                .Where(t => t.Id == query.TestProjectId)
                .SelectMany(t => t.TestPackages!)
                .AsNoTracking();

            var testPackagesDto = mapper.ProjectTo<TestPackageDto>(testPackages);
            
            return Task.FromResult(new Result
            {
                TestPackages = testPackagesDto
            });
        }
    }
}