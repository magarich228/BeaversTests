using AutoMapper;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.App.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestRunnerController.App.Queries;

public class GetAgentsQuery
{
    public class Query : IQuery<Result>;
    
    public class Result
    {
        public IEnumerable<TestAgentDto> Agents { get; init; } = null!;
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator() { }
    }
    
    public class Handler(
        ITestRunnerControllerContext db,
        IMapper mapper) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query query, CancellationToken cancellationToken = default)
        {
            var testAgents = db.TestAgents
                .AsNoTracking();
            
            return new Result
            {
                Agents = await mapper
                    .ProjectTo<TestAgentDto>(testAgents)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}