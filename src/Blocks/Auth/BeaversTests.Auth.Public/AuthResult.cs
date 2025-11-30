using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

public class AuthResult : OperationResult
{
    [JsonProperty("idToken")]
    public string? IdToken { get; set; }

    [JsonProperty("refreshToken")]
    public string? RefreshToken { get; set; }
    
    [JsonProperty("userId")]
    public string? UserId { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("expiresIn")]
    public DateTime? ExpiresIn { get; set; }
}