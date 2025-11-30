using Newtonsoft.Json;

namespace BeaversTests.Auth.FirebaseProvider;

internal class FirebaseTokenResponse
{
    [JsonProperty("expires_in")]
    public required string ExpiresIn { get; set; }
        
    [JsonProperty("token_type")]
    public required string TokenType { get; set; }
        
    [JsonProperty("refresh_token")]
    public required string RefreshToken { get; set; }
        
    [JsonProperty("id_token")]
    public required string IdToken { get; set; }
        
    [JsonProperty("user_id")]
    public required string UserId { get; set; }
        
    [JsonProperty("project_id")]
    public required string ProjectId { get; set; }
}