using BeaversTests.Common.Binary;
using BeaversTests.Common.S3.Abstractions;
using BeaversTests.TestsManager.App.Abstractions;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.Infrastructure.S3Access.Minio;

public class DriversStorageWriteService(
    IS3Provider s3Provider,
    ILogger<DriversStorageWriteService> logger): IDriversStorageWriteService
{
    public async Task AddTestDriverAsync(string testDriverKey, FileSystemEntity testPackageContent,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Adding test driver {testDriverKey} to S3", testDriverKey);
        
        var bucketName = GetBucketName(testDriverKey);
        
        await s3Provider.UploadToAsync(bucketName, testPackageContent, cancellationToken);
    }

    public async Task RemoveTestDriverAsync(string testDriverKey, CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Removing test driver {testDriverKey} from S3", testDriverKey);
        
        var bucketName = GetBucketName(testDriverKey);
        
        await s3Provider.RemoveBucketAsync(bucketName, cancellationToken);
    }
    
    private string GetBucketName(string testDriverKey) => $"{testDriverKey}-driver";
}