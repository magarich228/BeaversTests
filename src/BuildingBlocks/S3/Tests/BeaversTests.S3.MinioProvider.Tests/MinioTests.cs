using BeaversTests.Common.S3.Abstractions;
using Microsoft.Extensions.Logging;
using Minio;
using NUnit.Framework;

namespace BeaversTests.S3.MinioProvider.Tests;

[Category("Runtime")]
[TestFixture]
public class MinioTests
{
    private IS3Provider? _s3Provider = default;

    [SetUp]
    public void SetupProvider()
    {
        var loggerFactory = new LoggerFactory();
        var client = new MinioClient();
     
        // TODO: Add minio configuration
        
        _s3Provider = new MinioS3Provider(client, loggerFactory.CreateLogger<MinioS3Provider>());
    }
    
    [Test]
    public void GetAsyncTest()
    {
        
    }
}