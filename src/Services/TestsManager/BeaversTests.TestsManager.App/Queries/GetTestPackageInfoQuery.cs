using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestsManager.App.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Queries;

public abstract class GetTestPackageInfoQuery
{
    public class Query : IQuery<Result>
    {
        public required Guid TestPackageId { get; init; }
    }

    public class Result
    {
        
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITestsManagerContext db)
        {
            RuleFor(q => q.TestPackageId)
                .NotEmpty()
                .MustAsync(async (id, token) => 
                    await db.TestPackages.AnyAsync(t => t.Id == id, token))
                .WithMessage("Test package with this id does not exist");
        }
    }

    public class Handler : IQueryHandler<Query, Result>
    {
        public Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}