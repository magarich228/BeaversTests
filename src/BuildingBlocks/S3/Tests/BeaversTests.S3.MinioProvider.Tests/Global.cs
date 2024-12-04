using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace BeaversTests.S3.MinioProvider.Tests;

[SetUpFixture]
public class Global
{
    public static IConfiguration Configuration { get; private set; } = null!;
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [OneTimeSetUp]
    public void Setup()
    {
        SetupConfiguration();
        SetupServices();
    }
    
    private void SetupConfiguration()
    {
        var configurationBuilder = new ConfigurationBuilder();
        
        var configuration = configurationBuilder.AddJsonFile("appsettings.json")
            .Build();

        Configuration = configuration;
    }

    private void SetupServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();
        serviceCollection.AddS3MinioProvider(Configuration);
        
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}