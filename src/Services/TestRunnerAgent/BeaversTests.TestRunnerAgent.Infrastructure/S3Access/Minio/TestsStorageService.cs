using BeaversTests.TestRunnerAgent.App.Abstractions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace BeaversTests.TestRunnerAgent.Infrastructure.S3Access.Minio;

public class TestsStorageService(
    IMinioClient minioClient,
    ILogger<TestsStorageService> logger) : ITestsStorageReadService
{
    public async Task<IDictionary<string, byte[]>> GetTestPackageAsync(
        Guid testPackageId, 
        CancellationToken cancellationToken = default)
    {
        var bucketName = GetBucketName(testPackageId);
        
        if (!await minioClient.BucketExistsAsync(
                new BucketExistsArgs()
                    .WithBucket(bucketName), 
                cancellationToken))
        {
            // throw new ApplicationException("Bucket with this testPackageId does not exists");
        }

        var testPackageItems = minioClient.ListObjectsEnumAsync(
            new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithRecursive(true),
            cancellationToken);

        var testPackageItemData = new Dictionary<string, byte[]>();
        
        await foreach (var testPackageItem in testPackageItems)
        {
            var objectKey = testPackageItem.Key;
            
            var stat = await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithCallbackStream(async (stream, token) => 
                        testPackageItemData.Add(objectKey, await GetObjectData(stream, token))),
                cancellationToken);
            
            // var putTestPackageItemArgs = new PutObjectArgs()
            //     .WithBucket(bucketName)
            //     .WithObject(fullPath)
            //     .WithObjectSize(streamData.Length)
            //     .WithContentType(TestPackageItemContentType)
            //     .WithStreamData(streamData);
        }

        logger.LogInformation("Test package {TestPackageId} was downloaded", testPackageId);
        
        return testPackageItemData;
    }
    
    private string GetBucketName(Guid assemblyId) => $"tests-{assemblyId}";
    
    private async Task<byte[]> GetObjectData(
        Stream objectStream, 
        CancellationToken cancellationToken)
    {
        // TODO: Убрать MemoryStream
        await using var ms = new MemoryStream();
        
        await objectStream.CopyToAsync(ms, cancellationToken);
        await objectStream.FlushAsync(cancellationToken);

        await objectStream.DisposeAsync();

        return ms.ToArray();
    }
}