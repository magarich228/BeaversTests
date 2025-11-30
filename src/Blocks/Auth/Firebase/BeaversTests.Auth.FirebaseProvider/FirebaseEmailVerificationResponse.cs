using Newtonsoft.Json;

namespace BeaversTests.Auth.FirebaseProvider;

internal class FirebaseEmailVerificationResponse
{
    [JsonProperty("email")]
    public required string Email { get; set; }
}