namespace BeaversTests.Client;

public abstract class BeaversServiceClientBase : IAsyncDisposable, IDisposable
{
    protected BeaversServiceClientBase(Configuration configuration)
    {
        Configuration = configuration;
        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(Configuration.ApiGatewayUrl)
        };
    }

    protected readonly HttpClient HttpClient;
    public Configuration Configuration { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            HttpClient.Dispose();
        }
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        HttpClient.Dispose();
        return ValueTask.CompletedTask;
    }

    ~BeaversServiceClientBase()
    {
        Dispose(false);
    }
}