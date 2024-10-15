namespace BeaversTests.Client
{
    public abstract class BeaversServiceClientBase : IDisposable
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
        protected Configuration Configuration { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                HttpClient.Dispose();
            }
        }

        ~BeaversServiceClientBase()
        {
            Dispose(false);
        }
    }
}