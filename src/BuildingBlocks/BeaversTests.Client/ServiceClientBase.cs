namespace BeaversTests.Client;

public abstract class ServiceClientBase
{
    protected ServiceClientBase()
    {
        HttpClient = new HttpClient();
        _configuration = new Configuration();
    }

    private Configuration _configuration;
    protected readonly HttpClient HttpClient;
    
    public Configuration Configuration
    {
        get { return _configuration; }
        set
        {
            _configuration = value;
        }
    }
    
    
}