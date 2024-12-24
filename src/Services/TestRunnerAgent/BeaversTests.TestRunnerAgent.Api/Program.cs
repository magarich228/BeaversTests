using BeaversTests.Common.Binary;
using BeaversTests.TestRunner;
using BeaversTests.TestRunnerAgent.Api;
using BeaversTests.TestRunnerAgent.App;
using BeaversTests.TestRunnerAgent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddTestRunnerAgentInfrastructure(configuration);
services.AddTestRunnerControllerApp(configuration);
services.AddApi();

services.AddHostedService<TaskEngine>();

var app = builder.Build();

app.MapPost("/validate-driver", async (c) =>
{
    using var runnerClient = c.RequestServices.GetRequiredService<RunnerClient>();

    var command = new DriverValidationCommand()
    {
        DriverKey = "Test",
        AgId = Guid.NewGuid(),
        Driver = TestDriverContentFactory.CreateFromDirectory(
            @"C:\Users\kiril\RiderProjects\TMSNet\src\BuildingBlocks\Drivers\BeaversTests.NUnit.Driver\bin\Release\net8.0\publish")
    };
    
    var result = await runnerClient.SendAsync<DriverValidationCommand.Result>(command);
    
    Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result));
});

app.MapGet("/", () => "Alive.");
app.UseApi();

using (var scope = app.Services.CreateScope())
{
    var appLifetime = scope.ServiceProvider.GetRequiredService<IHostApplicationLifetime>();

    appLifetime.ApplicationStopping.Register(LifetimeActions.OnStopping, app);
    appLifetime.ApplicationStarted.Register(LifetimeActions.OnStarted, app);
}

await app.RunAsync();

// TODO: разгрести референсы для сервисов, удалить лишние Nuget пакеты

// TODO: Remove
public class TestDriverContentFactory
{
    public static TestDriverContent CreateFromDirectory(string path)
    {
        var dirInfo = new DirectoryInfo(path);

        var subDirs = new List<BeaversTestsDirectory>();
        var files = new List<BeaversTestsFile>();
        
        SetFiles(files, dirInfo.EnumerateFiles());
        SetDirectories(subDirs, dirInfo.EnumerateDirectories());
        
        var entity = new TestDriverContent()
        {
            Directories = subDirs,
            Files = files
        };

        return entity;
    }

    private static void SetDirectories(List<BeaversTestsDirectory> directories, IEnumerable<DirectoryInfo> dirInfos)
    {
        foreach (var dirInfo in dirInfos)
        {
            var subDirs = new List<BeaversTestsDirectory>();
            var files = new List<BeaversTestsFile>();
            
            SetFiles(files, dirInfo.EnumerateFiles());
            SetDirectories(subDirs, dirInfo.EnumerateDirectories());
            
            directories.Add(new BeaversTestsDirectory()
            {
                DirectoryName = dirInfo.Name,
                Directories = subDirs,
                TestFiles = files
            });
        }
    }
    
    private static void SetFiles(List<BeaversTestsFile> files, IEnumerable<FileInfo> fileInfos)
    {
        foreach (var fileInfo in fileInfos)
        {
            var content = File.ReadAllBytes(fileInfo.FullName);
            
            files.Add(new BeaversTestsFile
            {
                Name = fileInfo.Name,
                Content = content,
                Length = content.Length,
                MediaType = "application/octet-stream"
            });
        }
    }
}