using System.Reflection;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestsManager.App;

public static class DependencyInjection
{
    public static IServiceCollection AddTestsManagerApp(this IServiceCollection services)
    {
        var executingAssembly = Assembly.GetExecutingAssembly();
        
        services.AddFluentValidation(new []{ executingAssembly });

        services.AddAutoMapper(conf => conf.AddMaps(executingAssembly));
        
        services.AddMediatR(conf => conf.RegisterServicesFromAssembly(executingAssembly));
        
        return services;
    }
}