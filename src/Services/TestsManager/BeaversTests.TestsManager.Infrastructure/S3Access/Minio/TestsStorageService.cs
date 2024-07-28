using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Exceptions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace BeaversTests.TestsManager.Infrastructure.S3Access.Minio;

// TODO: Сделать фасадом и выделить?
// TODO: Абстрагироваться от Minio.
// TODO: Организовать бакеты по проектам.
public class TestsStorageService(
    IMinioClient minioClient,
    ILogger<TestsStorageService> logger) : ITestsStorageService
{
    private const string TestPackageItemContentType = "application/octet-stream";

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
            throw new ApplicationException("Bucket with this testPackageId does not exists");
        }

        var testPackageItems = minioClient.ListObjectsEnumAsync(
            new ListObjectsArgs().WithBucket(bucketName),
            cancellationToken);

        var testPackageItemDatas = new Dictionary<string, byte[]>();
        
        await foreach (var testPackageItem in testPackageItems)
        {
            var objectKey = testPackageItem.Key;
            
            var stat = await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithCallbackStream(async (stream, token) => 
                        testPackageItemDatas.Add(objectKey, await GetObjectData(stream, token))),
                cancellationToken);
        }

        return testPackageItemDatas;
    }
    
    public async Task AddTestPackageAsync(
        Guid testPackageId,
        IEnumerable<byte[]> testAssemblies,
        IEnumerable<string> assemblyPaths,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(testAssemblies, nameof(testAssemblies));
        ArgumentNullException.ThrowIfNull(assemblyPaths, nameof(assemblyPaths));

        var testAssembliesList = testAssemblies as List<byte[]> ?? testAssemblies.ToList();
        var assemblyPathsList = assemblyPaths as List<string> ?? assemblyPaths.ToList();

        if (testAssembliesList.Count != assemblyPathsList.Count)
        {
            throw new ArgumentException("Test assemblies and assembly paths count must be equal");
        }

        var bucketName = GetBucketName(testPackageId);

        logger.LogInformation(
            "Test package {testPackageId} bucket name: {BucketName}", 
            testPackageId, 
            bucketName);
        
        // bug https://github.com/minio/minio-dotnet/issues/1041
        // find stable version of minio or choose another client
        if (await minioClient.BucketExistsAsync(
                new BucketExistsArgs()
                    .WithBucket(bucketName), 
                cancellationToken))
        {
            //throw new ApplicationException("Bucket with this testPackageId already exists");
        }

        // TODO: Configure bucket access, lifecycle, versioning...
        await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), cancellationToken);

        for (int i = 0; i < testAssembliesList.Count; i++)
        {
            using var streamData = new MemoryStream(testAssembliesList[i]);

            var putTestPackageItemArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(assemblyPathsList[i])
                .WithObjectSize(streamData.Length)
                .WithContentType(TestPackageItemContentType)
                .WithStreamData(streamData);
            
            _ = await minioClient.PutObjectAsync(putTestPackageItemArgs, cancellationToken);
        }
    }
    
    public async Task RemoveTestPackageAsync(
        Guid testPackageId, 
        CancellationToken cancellationToken = default)
    {
        var bucketName = GetBucketName(testPackageId);

        var items = minioClient.ListObjectsEnumAsync(new ListObjectsArgs()
            .WithBucket(bucketName)
            .WithRecursive(true), cancellationToken)
            .ToBlockingEnumerable();
        
        var deleteErrors = await minioClient.RemoveObjectsAsync(
            new RemoveObjectsArgs()
                .WithBucket(bucketName)
                .WithObjects(items.Select(i => i.Key)
                    .ToList()), 
            cancellationToken);
        
        if (deleteErrors.Any())
        {
            // TODO: Log errors
            throw new TestsManagerException("Failed to remove test assemblies from S3");
        }
        
        // TODO: learn locks and remove buckets
        await minioClient.RemoveBucketAsync(
            new RemoveBucketArgs()
                .WithBucket(bucketName), 
            cancellationToken);
    }

    private string GetBucketName(Guid assemblyId) => $"tests-{assemblyId}";

    private async Task<byte[]> GetObjectData(
        Stream objectStream, 
        CancellationToken cancellationToken)
    {
        await using var ms = new MemoryStream();

        objectStream.Position = 0;
        await objectStream.CopyToAsync(ms, cancellationToken);
        await objectStream.FlushAsync(cancellationToken);

        await objectStream.DisposeAsync();

        return ms.ToArray();
    }
}