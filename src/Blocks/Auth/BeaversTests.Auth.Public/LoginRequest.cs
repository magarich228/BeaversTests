using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

public class LoginRequest(string email, string password)
{
    [JsonProperty("email")]
    public string Email { get; set; } = email;

    [JsonProperty("password")]
    public string Password { get; set; } = password;
}