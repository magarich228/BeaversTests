using System;

namespace BeaversTests.Client.Http.Configuration;

public class AuthTokens
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
}