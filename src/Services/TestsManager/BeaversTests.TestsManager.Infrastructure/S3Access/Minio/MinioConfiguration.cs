namespace BeaversTests.TestsManager.Infrastructure.S3Access.Minio;

public class MinioConfiguration
{
    public string Endpoint { get; set; } = null!;
    public string AccessKey { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public bool UseSsl { get; set; } = false;
}