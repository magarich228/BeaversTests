using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

public class RegisterRequest(string email, string password, string displayName)
{
    [JsonProperty("email")]
    public string Email { get; set; } = email;

    [JsonProperty("password")]
    public string Password { get; set; } = password;

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = displayName;
}