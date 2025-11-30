using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

public class RefreshTokenRequest(string refreshToken)
{
    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; } = refreshToken;
}