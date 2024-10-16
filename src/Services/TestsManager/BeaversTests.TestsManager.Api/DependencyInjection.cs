using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestsManager.Api.Dtos;
using BeaversTests.TestsManager.Api.Middlewares;
using BeaversTests.TestsManager.Api.Services;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Events.TestPackage;
using BeaversTests.TestsManager.Infrastructure.DataAccess;

namespace BeaversTests.TestsManager.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(
            typeof(TestPackageBase).Assembly,
            typeof(TestsManagerContext).Assembly));
        services.AddTransient<ITestPackageContentExtractor<TestPackageZipDto>, ZipTestPackageContentExtractor>();
        
        return services;
    }
    
    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationErrorMiddleware>();

        var messageBroker = app.ApplicationServices.GetRequiredService<IMessageBroker>();

        messageBroker.SubscribeAsync<TestPackageAddedEvent>();
        
        return app;
    }
}