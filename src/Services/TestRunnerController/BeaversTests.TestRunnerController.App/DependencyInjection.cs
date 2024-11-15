using System.Reflection;
using BeaversTests.Common.CQRS;
using BeaversTests.TestRunnerController.Core;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestRunnerController.App;

public static class DependencyInjection
{
    public static IServiceCollection AddTestRunnerControllerApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCqrs(configuration);
        
        var executingAssembly = Assembly.GetExecutingAssembly();
        
        services.AddFluentValidation(new []{ executingAssembly });

        services.AddAutoMapper(conf => conf.AddMaps(executingAssembly));

        services.AddScoped<ITestAgentsPool, TestAgentsPool>();
        
        return services;
    }
}