using System.Reflection;
using BeaversTests.Common.CQRS;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestRunnerAgent.App;

public static class DependencyInjection
{
    public static IServiceCollection AddTestRunnerControllerApp(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCqrsCore(configuration);
        
        var executingAssembly = Assembly.GetExecutingAssembly();
        
        services.AddFluentValidation(new []{ executingAssembly });

        services.AddAutoMapper(conf => conf.AddMaps(executingAssembly));
        
        return services;
    }
}