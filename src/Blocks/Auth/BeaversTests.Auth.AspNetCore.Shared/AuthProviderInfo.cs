namespace BeaversTests.Auth.AspNetCore.Shared;

public class AuthProviderInfo
{
    public required string Authority { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
}