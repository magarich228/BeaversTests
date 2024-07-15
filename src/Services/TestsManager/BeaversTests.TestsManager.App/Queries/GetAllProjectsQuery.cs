using AutoMapper;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Queries;

public abstract class GetAllProjectsQuery
{
    public class Query : IQuery<Result> { }
    
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
        IMapper mapper) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query query, CancellationToken cancellationToken)
        {
            var testProjects = db.TestProjects.AsNoTracking();
            
            return new Result
            {
                TestProjects = await mapper
                    .ProjectTo<TestProjectDto>(testProjects)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}