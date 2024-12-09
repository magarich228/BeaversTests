using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.Api.Shared;

public static class DependencyInjection
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        return services;
    }
}