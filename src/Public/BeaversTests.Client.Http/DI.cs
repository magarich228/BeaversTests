using System;
using System.Net.Http;
using BeaversTests.Client.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Extensions.Http;

namespace BeaversTests.Client.Http;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBeaversTestsHttpClient(
        this IServiceCollection services,
        Action<BeaversTestsClientOptions>? configureOptions = null)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));

        // Configure options
        var options = new BeaversTestsClientOptions();
        configureOptions?.Invoke(options);
        services.TryAddSingleton(options);

        // Register HTTP client with Polly policies
        services.AddHttpClient("BeaversTests", client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = options.Timeout;
                client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
                client.DefaultRequestHeaders.Add("X-Client-Version", options.ClientVersion);
                client.DefaultRequestHeaders.Add("X-Client-Name", options.ClientName);
            })
            .AddPolicyHandler(GetRetryPolicy(options))
            .AddPolicyHandler(GetCircuitBreakerPolicy());

        // Register services
        services.TryAddSingleton<IBeaversTestsClientFactory, BeaversTestsClientFactory>();
        services.TryAddTransient<IBeaversTestsAuthClient>(provider =>
        {
            var factory = provider.GetRequiredService<IBeaversTestsClientFactory>();
            return factory.CreateAuthClient();
        });

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(BeaversTestsClientOptions options)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => (int)msg.StatusCode >= 500)
            .WaitAndRetryAsync(
                options.MaxRetryAttempts,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + options.RetryDelay);
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30));
    }
}