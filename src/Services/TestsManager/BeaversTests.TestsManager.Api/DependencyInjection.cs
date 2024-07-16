using BeaversTests.TestsManager.Api.Middlewares;

namespace BeaversTests.TestsManager.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services
            .AddControllers();
        
        return services;
    }
    
    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationMiddleware>();
        
        return app;
    }
}