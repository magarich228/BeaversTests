using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestDrivers.Internal;

// TODO: Split the assembly into a package for driver development and a assembly for services
public static class DependencyInjection
{
    public static IServiceCollection AddTestDrivers(this IServiceCollection services)
    {
        services.AddSingleton<TestDriversRegistry>();
        services.AddSingleton<TestDriversResolver>();
        
        return services;
    }
}