using System.Net.Http;
using BeaversTests.Client.Http.Configuration;
using Microsoft.Extensions.Logging;

namespace BeaversTests.Client.Http;

public class BeaversTestsClientFactory(
    IHttpClientFactory httpClientFactory,
    BeaversTestsClientOptions defaultOptions,
    ILoggerFactory loggerFactory)
    : IBeaversTestsClientFactory
{
    public IBeaversTestsAuthClient CreateAuthClient()
    {
        return CreateAuthClient(defaultOptions);
    }

    // TODO: Потестить, реализовать один экземпляр конфигурации на каждый тип клиента.
    public IBeaversTestsAuthClient CreateAuthClient(BeaversTestsClientOptions options)
    {
        var httpClient = httpClientFactory.CreateClient("BeaversTests");
        var logger = loggerFactory.CreateLogger<BeaversTestsAuthClient>();

        return new BeaversTestsAuthClient(httpClient, options, logger);
    }
}