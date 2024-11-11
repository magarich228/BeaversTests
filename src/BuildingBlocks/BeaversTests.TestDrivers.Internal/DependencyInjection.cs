using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestDrivers.Internal;

public static class DependencyInjection
{
    public static IServiceCollection AddTestDrivers(this IServiceCollection services)
    {
        services.AddSingleton<TestDriversRegistry>();
        services.AddSingleton<TestDriversResolver>();
        
        return services;
    }
}