using BeaversTests.Client.Http.Configuration;

namespace BeaversTests.Client.Http;

public interface IBeaversTestsClientFactory
{
    IBeaversTestsAuthClient CreateAuthClient();
    IBeaversTestsAuthClient CreateAuthClient(BeaversTestsClientOptions options);
}