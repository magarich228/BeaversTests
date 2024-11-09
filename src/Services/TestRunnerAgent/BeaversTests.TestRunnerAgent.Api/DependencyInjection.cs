using BeaversTests.Api.Shared.Middlewares;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerAgent.Infrastructure.S3Access.Minio;

namespace BeaversTests.TestRunnerAgent.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(
            typeof(BeaversTests.TestRunnerAgent.App.DependencyInjection).Assembly,
            typeof(TestsStorageService).Assembly));
        
        return services;
    }
    
    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationErrorMiddleware>();
        
        return app;
    }
}