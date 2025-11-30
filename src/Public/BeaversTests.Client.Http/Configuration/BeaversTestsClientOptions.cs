using System;

namespace BeaversTests.Client.Http.Configuration;

public class BeaversTestsClientOptions
{
    public string BaseUrl { get; set; } = "http://localhost:5068";
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(2);
    public string UserAgent { get; set; } = "BeaversTests.Client.Http/0.0.1";
        
    // Authentication
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; } // TODO: implement
        
    // Headers
    public string ClientVersion { get; set; } = "0.0.1";
    public string ClientName { get; set; } = "BeaversTests.Client";
}