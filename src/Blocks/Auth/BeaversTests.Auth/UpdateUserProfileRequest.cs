namespace BeaversTests.Auth;

// TODO: move to public
public class UpdateUserProfileRequest
{
    // TODO: public string? Email { get; init; } в AuthService
    // TODO: public string? PhoneNumber { get; init; }
    public string? DisplayName { get; init; }
    public string? PhotoUrl { get; init; }
}