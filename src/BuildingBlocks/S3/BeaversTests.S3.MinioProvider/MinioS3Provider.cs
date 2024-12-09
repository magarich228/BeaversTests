using BeaversTests.Common.Binary;
using BeaversTests.Common.S3.Abstractions;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace BeaversTests.S3.MinioProvider;

// TODO: Организовать бакеты по проектам?
public class MinioS3Provider(
    IMinioClient minioClient,
    ILogger<MinioS3Provider> logger) : IS3Provider
{
    private const string TestPackageItemContentType = "application/octet-stream";
    
    public async Task<TEntity> GetAsync<TEntity>(string bucketName, CancellationToken cancellationToken = default)
        where TEntity : FileSystemEntity, new()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));

        var resolver = new FileSystemEntityFileResolver();
        
        // bug https://github.com/minio/minio-dotnet/issues/1041
        // find stable version of minio or choose another client
        if (!await minioClient.BucketExistsAsync(
                new BucketExistsArgs()
                    .WithBucket(bucketName), 
                cancellationToken))
        {
            throw new ApplicationException($"Bucket with this name ({bucketName}) does not exists.");
        }
        
        var items = minioClient.ListObjectsEnumAsync(
            new ListObjectsArgs()
                .WithBucket(bucketName)
                .WithRecursive(true),
            cancellationToken);

        var context = new FileSystemEntityContext();
        
        await foreach (var item in items)
        {
            var objectKey = item.Key;
            
            var stat = await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectKey)
                    .WithCallbackStream(async (stream, token) => await resolver.ResolveFromAsync(
                        context,
                        item,
                        stream,
                        token)),
                cancellationToken);
        }

        logger.LogInformation("Item from {BucketName} was downloaded", bucketName);
        
        return context.ToEntity<TEntity>();
    }

    public async Task UploadToAsync(string bucketName, FileSystemEntity @object, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));
        ArgumentNullException.ThrowIfNull(@object, nameof(@object));

        logger.LogInformation(
            "New bucket name: {BucketName}", 
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

        await UploadInternalAsync(@object, bucketName, cancellationToken);
    }

    public async Task RemoveBucketAsync(string bucketName, CancellationToken cancellationToken = default)
    {
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
            throw new BeaversTestsS3Exception("Failed to remove test assemblies from S3");
        }
        
        // TODO: learn locks and remove buckets
        await minioClient.RemoveBucketAsync(
            new RemoveBucketArgs()
                .WithBucket(bucketName), 
            cancellationToken);
        
        logger.LogInformation("Bucket {BucketName} was removed", bucketName);
    }
    
    public void Dispose()
    {
        minioClient.Dispose();
    }
    
    private async Task UploadInternalAsync(FileSystemEntity content, string bucketName, CancellationToken cancellationToken)
    {
        var root = new BeaversTestsDirectory()
        {
            DirectoryName = string.Empty, //root
            Directories = content.Directories,
            TestFiles = content.Files
        };

        await AddDirectoryAsync(root, string.Empty, bucketName, cancellationToken);
    }
    
    private async Task AddDirectoryAsync(BeaversTestsDirectory rootDirectory, string previousPath, string bucketName, CancellationToken cancellationToken)
    {
        foreach (var file in rootDirectory.TestFiles)
        {
            await AddFileAsync(file, previousPath, bucketName, cancellationToken);
        }
        
        foreach (var subDir in rootDirectory.Directories)
        {
            await AddDirectoryAsync(subDir, $"{previousPath}/{subDir.DirectoryName}", bucketName, cancellationToken);
        }
    }
    
    private async Task AddFileAsync(BeaversTestsFile file, string dirPath, string bucketName, CancellationToken cancellationToken)
    {
        using var streamData = new MemoryStream(file.Content);
        var fullPath = $"{dirPath}/{file.Name}";//Path.Combine(dirPath, file.Name);

        logger.LogDebug("Object {ObjectKey} with size {ObjectSize} put it {BucketName}", 
            fullPath, streamData.Length, bucketName);
        
        if (file.Length == 0)
        {
            logger.LogWarning("Object {ObjectKey} with size {ObjectSize} for bucket {BucketName} skipped because of zero size", 
                fullPath, streamData.Length, bucketName);
            return;
        }
        
        var putTestPackageItemArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(fullPath)
            .WithObjectSize(streamData.Length)
            .WithContentType(TestPackageItemContentType)
            .WithStreamData(streamData);
            
        var response = await minioClient.PutObjectAsync(putTestPackageItemArgs, cancellationToken);
        
        logger.LogDebug("Argument file {fullPath} size: {ObjectSize} and actual size: {ActualObjectSize}", 
            fullPath, streamData.Length, response.Size);
    }
}