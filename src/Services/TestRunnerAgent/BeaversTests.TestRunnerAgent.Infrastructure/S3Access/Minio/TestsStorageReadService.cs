using BeaversTests.Common.Binary;
using BeaversTests.Common.S3.Abstractions;
using BeaversTests.TestRunnerAgent.App.Abstractions;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerAgent.Infrastructure.S3Access.Minio;

public class TestsStorageReadService(
    IS3Provider s3Provider,
    ILogger<TestsStorageReadService> logger) : ITestsStorageReadService
{
    public async Task<TestPackageContent> GetTestPackageAsync(Guid testPackageId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting test package {testPackageId} from S3", testPackageId);
        
        var bucketName = GetBucketName(testPackageId);
        
        return await s3Provider.GetAsync<TestPackageContent>(bucketName, cancellationToken);
    }

    // TODO: В общее?
    private string GetBucketName(Guid testPackageId)
    {
        return $"{testPackageId}-tests";
    }
}