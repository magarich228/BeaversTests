using System.Text.Json.Serialization;

namespace BeaversTests.Auth.FirebaseProvider;

internal class FirebaseSignInResponse : FirebaseSignInResponseBase
{
    [JsonPropertyName("registered")]
    public required bool Registered { get; set; }
}