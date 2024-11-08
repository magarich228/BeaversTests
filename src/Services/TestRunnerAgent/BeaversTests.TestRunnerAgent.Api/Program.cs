using BeaversTests.TestRunnerAgent.Api;
using BeaversTests.TestRunnerAgent.App;
using BeaversTests.TestRunnerAgent.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddTestRunnerAgentInfrastructure(configuration);
services.AddTestRunnerControllerApp(configuration);
services.AddApi();

var app = builder.Build();

app.MapGet("/", () => "Alive.");
app.UseApi();

app.Run();

// TODO: подумать над окружениями для тестов в агенте
// TODO: разгрести референсы для сервисов, удалить лишние Nuget пакеты