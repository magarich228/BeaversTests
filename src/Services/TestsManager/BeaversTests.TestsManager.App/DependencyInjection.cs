using System.Reflection;
using BeaversTests.Common.CQRS;
using BeaversTests.TestDrivers.Internal;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestsManager.App;

public static class DependencyInjection
{
    public static IServiceCollection AddTestsManagerApp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCqrs(configuration);
        
        var executingAssembly = Assembly.GetExecutingAssembly();
        
        services.AddFluentValidation(new []{ executingAssembly });

        services.AddAutoMapper(conf => conf.AddMaps(executingAssembly));
        
        services.AddMediatR(conf => conf.RegisterServicesFromAssembly(executingAssembly));

        services.AddTransient<TestPackageExtractor>();
        
        services.AddTestDrivers();
        
        return services;
    }
}