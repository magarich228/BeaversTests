using System.Text.Json.Serialization;

namespace BeaversTests.Auth.FirebaseProvider;

internal class FirebaseSignInResponseBase
{
    [JsonPropertyName("idToken")]
    public required string IdToken { get; set; }
    
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; set; }
    
    [JsonPropertyName("expiresIn")]
    public required string ExpiresIn { get; set; }
    
    [JsonPropertyName("localId")]
    public required string LocalId { get; set; }
}