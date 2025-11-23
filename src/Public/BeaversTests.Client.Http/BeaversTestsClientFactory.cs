using System.Net.Http;
using BeaversTests.Client.Http;
using BeaversTests.Client.Http.Configuration;
using Microsoft.Extensions.Logging;

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

    public IBeaversTestsAuthClient CreateAuthClient(BeaversTestsClientOptions options)
    {
        var httpClient = httpClientFactory.CreateClient("BeaversTests");
        var logger = loggerFactory.CreateLogger<BeaversTestsAuthClient>();

        return new BeaversTestsAuthClient(httpClient, options, logger);
    }
}