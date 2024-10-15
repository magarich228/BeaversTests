using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.Api.Middlewares;
using BeaversTests.TestsManager.Api.Services;
using BeaversTests.TestsManager.App.Abstractions;

namespace BeaversTests.TestsManager.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddTransient<ITestPackageContentExtractor<TestPackageZipDto>, ZipTestPackageContentExtractor>();
        
        return services;
    }
    
    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationErrorMiddleware>();
        
        return app;
    }
}