using BeaversTests.Common.Binary;

namespace BeaversTests.Common.S3.Abstractions;

public interface IS3Provider : IDisposable
{
    Task<FileSystemEntity> GetAsync<TEntity>(string bucketName, CancellationToken cancellationToken = default)
        where TEntity : FileSystemEntity, new();
    Task UploadToAsync(string bucketName, FileSystemEntity @object, CancellationToken cancellationToken = default);
    Task RemoveBucketAsync(string bucketName, CancellationToken cancellationToken = default);
}