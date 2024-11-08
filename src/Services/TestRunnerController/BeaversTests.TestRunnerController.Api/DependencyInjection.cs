using BeaversTests.Api.Shared.Middlewares;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerController.Infrastructure.DataAccess;
using BeaversTests.TestsManager.Events.TestPackage;

namespace BeaversTests.TestRunnerController.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddMediatR(conf => conf.RegisterServicesFromAssemblies(
            typeof(BeaversTests.TestRunnerController.App.DependencyInjection).Assembly,
            typeof(TestRunnerControllerContext).Assembly));
        
        return services;
    }
    
    public static IApplicationBuilder UseApi(this IApplicationBuilder app)
    {
        app.UseMiddleware<ValidationErrorMiddleware>();

        var messageBroker = app.ApplicationServices.GetRequiredService<IMessageBroker>();

        messageBroker.SubscribeAsync<TestPackageAddedEvent>().Wait();
        messageBroker.SubscribeAsync<TestRunnerPreparedEvent>().Wait();
        messageBroker.SubscribeAsync<TestRunnerFinalizedEvent>().Wait();
        
        return app;
    }
}