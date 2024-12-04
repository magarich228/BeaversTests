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

        var resultEntity = await minioProvider.GetAsync(TestBucketName);
        
        Assert.That(resultEntity, Is.Not.Null);
        
        await minioProvider.RemoveBucketAsync(TestBucketName);
        
        // TODO: get async
        Assert.Pass();
    }
    
    // [Test]
    // public async Task GetAsyncTest()
    // {
    //     using var minioProvider = Global.ServiceProvider.GetRequiredService<IS3Provider>();
    //
    //     
    // }
    //
    // [Test]
    // public async Task RemoveBucketAsyncTest()
    // {
    //     using var minioProvider = Global.ServiceProvider.GetRequiredService<IS3Provider>();
    //     
    //     
    // }
}