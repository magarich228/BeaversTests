using BeaversTests.Common.S3.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BeaversTests.S3.MinioProvider.Tests;

[Category("Runtime")]
[TestFixture]
public class MinioTests
{
    private const string TestBucketName = "test-bucket";
    
    [Test]
    public async Task UploadGetRemoveAsyncTest()
    {
        using var minioProvider = Global.ServiceProvider.GetRequiredService<IS3Provider>();
        var entity = DirectoryEntity.Create();

        await minioProvider.UploadToAsync(TestBucketName, entity);

        var resultEntity = await minioProvider.GetAsync<DirectoryEntity>(TestBucketName);
        
        Assert.That(resultEntity, Is.Not.Null);
        Assert.That(resultEntity.Files.Count(), Is.EqualTo(entity.Files.Count()));
        Assert.That(resultEntity.Directories.Count(), Is.EqualTo(entity.Directories.Count())); // TODO: нормальные проверки.
        
        await minioProvider.RemoveBucketAsync(TestBucketName);
        
        // TODO: get async
        Assert.Pass();
    }
}