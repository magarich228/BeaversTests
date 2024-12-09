using BeaversTests.Common.Binary;
using BeaversTests.Common.S3.Abstractions;
using BeaversTests.TestsManager.App.Abstractions;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.Infrastructure.S3Access.Minio;

public class TestsStorageWriteService(
    IS3Provider s3Provider,
    ILogger<TestsStorageWriteService> logger) : ITestsStorageWriteService
{
    public async Task AddTestPackageAsync(
        Guid testPackageId, 
        FileSystemEntity testPackageContent,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug($"Adding test package {testPackageId} to S3");

        var bucketName = GetBucketName(testPackageId);
        
        await s3Provider.UploadToAsync(bucketName, testPackageContent, cancellationToken);
    }

    public async Task RemoveTestPackageAsync(
        Guid testPackageId, 
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug($"Removing test package {testPackageId} from S3");
        
        var bucketName = GetBucketName(testPackageId);
        
        await s3Provider.RemoveBucketAsync(bucketName, cancellationToken);
    }
    
    private string GetBucketName(Guid testPackageId) => $"{testPackageId}-tests";
}