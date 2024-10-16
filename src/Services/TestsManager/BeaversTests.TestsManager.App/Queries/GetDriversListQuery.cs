using AutoMapper;
using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Queries;

public class GetDriversListQuery
{
    public class Query : IQuery<Result> { }
    
    public class Result
    {
        public required IEnumerable<TestDriverDto> TestDrivers { get; init; }
    }

    public class Handler(
        ITestsManagerContext db,
        IMapper mapper) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var testDrivers = db.TestDrivers
                .AsNoTracking();

            return new Result()
            {
                TestDrivers = await mapper
                    .ProjectTo<TestDriverDto>(testDrivers)
                    .ToListAsync(cancellationToken)
            };
        }
    }
}