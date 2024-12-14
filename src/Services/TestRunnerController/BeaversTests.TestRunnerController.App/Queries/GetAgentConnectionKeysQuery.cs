using AutoMapper;
using AutoMapper.QueryableExtensions;
using BeaversTests.Common.Application;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestRunnerController.App.Abstractions;
using BeaversTests.TestRunnerController.App.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.Queries;

public class GetAgentConnectionKeysQuery
{
    public class Query : IQuery<Result>
    {
    }

    public class Result
    {
        public required IEnumerable<AgentConnectionKeyDto> Keys { get; init; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            
        }
    }

    public class Handler(
        ITestRunnerControllerContext db,
        IMapper mapper,
        IUserService userService,
        ILogger<Handler> logger) : IQueryHandler<Query, Result>
    {
        public Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = userService.GetCurrentUserId();
            
            logger.LogDebug("Getting user controller keys for user {UserId}", userId);

            var keys = db.ControllerUserKeys
                .Where(k => k.OwnerId == userId)
                .AsNoTracking();

            return Task.FromResult(new Result()
            {
                Keys = keys.ProjectTo<AgentConnectionKeyDto>(mapper.ConfigurationProvider)
            });
        }
    }
}