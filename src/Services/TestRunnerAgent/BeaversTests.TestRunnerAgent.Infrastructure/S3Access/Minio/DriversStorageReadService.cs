using BeaversTests.Common.Binary;
using BeaversTests.Common.S3.Abstractions;
using BeaversTests.TestRunnerAgent.App.Abstractions;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerAgent.Infrastructure.S3Access.Minio;

public class DriversStorageReadService(
    IS3Provider s3Provider,
    ILogger<DriversStorageReadService> logger) : IDriversStorageReadService
{
    public Task<TestDriverContent> GetTestDriverAsync(string testDriverKey, Guid agId, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Getting test driver {testDriverKey} from S3", testDriverKey);
        
        var bucketName = GetBucketName(testDriverKey, agId);
        
        return s3Provider.GetAsync<TestDriverContent>(bucketName, cancellationToken);
    }

    private string GetBucketName(string testDriverKey, Guid agId) => $"{agId}-{testDriverKey.ToLower()}-driver";
}