using BeaversTests.Common.Binary;

namespace BeaversTests.Common.S3.Abstractions;

public interface IS3Provider
{
    Task GetAsync(string bucketName, CancellationToken cancellationToken = default);
    Task UploadToAsync(string bucketName, FileSystemEntity @object, CancellationToken cancellationToken = default);
    Task RemoveBucketAsync(string bucketName, CancellationToken cancellationToken = default);
}