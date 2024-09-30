using BeaversTests.Common.CQRS.Queries;
using BeaversTests.TestDrivers;
using BeaversTests.TestDrivers.Models;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
        TestDriversResolver testDriversResolver,
        ILogger<Handler> logger) : IQueryHandler<Query, Result>
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

            var testsExplorer = testDriversResolver.ResolveTestsExplorer(testPackage.TestDriverKey);

            // TODO: Временное решение, перенести в создание пакета
            var tempDirectory = new DirectoryInfo(Path.GetTempPath());
            var testPackageDirectory = tempDirectory.CreateSubdirectory(testPackageId.ToString());

            List<TestPackageTestSuiteDto> resultTestSuites = new();

            foreach (var testPackageItemPath in testPackageItems.Keys)
            {
                var testPackageItemFullName = Path.Combine(testPackageDirectory.FullName, testPackageItemPath);
                var fileInfo = new FileInfo(testPackageItemFullName);
                
                if (!(fileInfo.Directory?.Exists ?? false))
                {
                    fileInfo.Directory?.Create();
                }
                
                await using (var file =
                    File.Create(testPackageItemFullName))
                await using (var ms = new MemoryStream(testPackageItems[testPackageItemPath]))
                {
                    await ms.CopyToAsync(file, cancellationToken);
                    await ms.FlushAsync(cancellationToken);
                    await file.FlushAsync(cancellationToken);
                }

                IEnumerable<TestSuite> testSuites = null!;

                try
                {
                    testSuites = testsExplorer
                        .GetTestSuites(testPackageItemFullName);
                }
                catch (Exception exception)
                {
                    logger.LogInformation($"Missing test detection in the file ({testPackageItemFullName}) due to: {exception}", testPackageItemFullName, exception);
                    continue;
                }
                
                var testSuitesDtos = testSuites
                    .Select(s => new TestPackageTestSuiteDto
                    {
                        Name = s.Name,
                        Tests = s.Tests.Select(t => new TestPackageTestDto
                        {
                            Name = t.Name
                        })
                    }).ToList();

                if (testSuitesDtos.Any())
                {
                    logger.LogDebug("{File} Test suites found: {TestSuitesCount}", testPackageItemFullName, testSuitesDtos.Count);
                }
                
                resultTestSuites.AddRange(testSuitesDtos);
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