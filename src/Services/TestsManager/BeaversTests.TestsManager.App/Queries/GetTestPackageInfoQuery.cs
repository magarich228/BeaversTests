using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestDrivers;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
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
            var testPackageId = request.TestPackageId;

            var testPackage = await db.TestPackages.FirstOrDefaultAsync(
                t => t.Id == testPackageId,
                cancellationToken);

            if (testPackage is null)
            {
                return new Result();
            }

            var testPackageItems = await testsStorageService.GetTestPackageAsync(testPackageId, cancellationToken);

            var testsExplorer = testDriversResolver.ResolveTestsExplorer(testPackage.TestDriver.ToString());

            var tempDirectory = new DirectoryInfo(Path.GetTempPath());
            var testPackageDirectory = tempDirectory.CreateSubdirectory(testPackageId.ToString());

            List<TestPackageTestSuiteDto> resultTestSuites = new();

            foreach (var testPackageItemPath in testPackageItems.Keys)
            {
                var testPackageItemFullName = Path.Combine(testPackageDirectory.FullName, testPackageItemPath);

                await using (var file =
                    File.Create(testPackageItemFullName))
                await using (var ms = new MemoryStream(testPackageItems[testPackageItemPath]))
                {
                    await ms.CopyToAsync(file, cancellationToken);
                    await ms.FlushAsync(cancellationToken);
                    await file.FlushAsync(cancellationToken);
                }

                var testSuites = testsExplorer
                    .GetTestSuites(testPackageItemFullName)
                    .Select(s => new TestPackageTestSuiteDto
                    {
                        Name = s.Name,
                        Tests = s.Tests.Select(t => new TestPackageTestDto
                        {
                            Name = t.Name
                        })
                    });

                resultTestSuites.AddRange(testSuites);
            }

            return new Result
            {
                TestPackageItemsInfo = new TestPackageItemsInfoDto
                {
                    TestPackageId = testPackageId,
                    TestSuites = resultTestSuites
                }
            };
        }
    }
}