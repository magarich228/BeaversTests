using BeaversTests.TestsManager.App.Abstractions;
using Minio;

namespace BeaversTests.TestsManager.Infrastructure.S3Access.Minio;

public class TestsStorageService(IMinioClient minioClient) : ITestsStorageService
{
    public Task AddTestAssembly(string name, byte[] testProject, Guid projectId)
    {
        // TODO: Implement

        throw new NotImplementedException();
    }
}