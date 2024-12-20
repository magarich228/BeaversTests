using System.Reflection;
using BeaversTests.Common.CQRS;
using BeaversTests.Isolation;
using BeaversTests.TestRunnerAgent.Core;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestRunnerAgent.App;

public static class DependencyInjection
{
    private const string ControllerConnectionKeyConfigurationKey = "ControllerConnectionKey";

    public static IServiceCollection AddTestRunnerControllerApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var controllerConnectionKey = configuration[ControllerConnectionKeyConfigurationKey] ??
                                      throw new ApplicationException("Controller connection key not found. " +
                                                                     "Get the secret connection key from the test runner controller and paste it into the runner configuration" +
                                                                     $" with configuration key: {ControllerConnectionKeyConfigurationKey}");

        services.AddSingleton(new TestRunnerContext(controllerConnectionKey));

        services.AddCqrsCore(configuration);

        var executingAssembly = Assembly.GetExecutingAssembly();

        services.AddFluentValidation(new[] {executingAssembly});

        services.AddAutoMapper(conf => conf.AddMaps(executingAssembly));

        services.AddScoped<RunnerClient>();
        
        services.AddSingleton<TasksContainer>();
        services.AddSingleton<IsolationService>();

        return services;
    }
}