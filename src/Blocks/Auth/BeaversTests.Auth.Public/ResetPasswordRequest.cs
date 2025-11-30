using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

public class ResetPasswordRequest(string email)
{
    [JsonProperty("email")]
    public string Email { get; set; } = email;
}