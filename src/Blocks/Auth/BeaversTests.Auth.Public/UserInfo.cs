using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

// TODO: CreatedAt, LastLoginAt?
public class UserInfo(string userId, string email, string displayName, bool? emailVerified = null)
{
    [JsonProperty("userId")]
    public string UserId { get; set; } = userId;

    [JsonProperty("email")]
    public string Email { get; set; } = email;

    [JsonProperty("displayName")]
    public string DisplayName { get; set; } = displayName;

    [JsonProperty("emailVerified")]
    public bool? EmailVerified { get; set; } = emailVerified;
}