using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.Common.CQRS.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.Common.CQRS;

public static class DependencyInjection
{
    public static IServiceCollection AddCqrsBusses(this IServiceCollection services)
    {
        services.AddScoped<ICommandBus, CommandBus>();
        services.AddScoped<IQueryBus, QueryBus>();
        services.AddScoped<IEventBus, EventBus>();
        
        return services;
    }
}