using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

public class VerificationResult
{
    [JsonProperty("isAuthenticated")]
    public bool IsAuthenticated { get; set; }

    [JsonProperty("userInfo")]
    public UserInfo? UserInfo { get; set; }

    [JsonProperty("timestamp")]
    public DateTime Timestamp { get; set; }
}