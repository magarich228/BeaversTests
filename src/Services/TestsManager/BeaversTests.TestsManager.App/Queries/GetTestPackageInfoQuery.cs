using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestDrivers;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.Models.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestsManager.App.Queries;

public abstract class GetTestPackageInfoQuery
{
    public class Query : IQuery<Result>
    {
        public required Guid TestPackageId { get; init; }
    }

    public class Result
    {
        public TestPackageItemsInfoDto? TestPackageItemsInfo { get; init; }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator(ITestsManagerContext db)
        {
            // TODO: check relation to current user project
            
            RuleFor(q => q.TestPackageId)
                .NotEmpty()
                .MustAsync(async (id, token) => 
                    await db.TestPackages.AnyAsync(t => t.Id == id, token))
                .WithMessage("Test package with this id does not exist");
        }
    }

    public class Handler(
        ITestsManagerContext db,
        ITestsStorageService testsStorageService,
        TestDriversResolver testDriversResolver) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var testPackage = await db.TestPackages.FirstOrDefaultAsync(
                t => t.Id == request.TestPackageId, 
                cancellationToken);
            
            // TODO: get test package info logic
            
            if (testPackage is null)
            {
                return new Result();
            }
            
            var testsExplorer = testDriversResolver.ResolveTestsExplorer(testPackage.TestPackageType.ToString());

            // testsExplorer.GetTestSuites();
            
            return new Result();
        }
    }
}