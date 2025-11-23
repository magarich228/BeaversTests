namespace BeaversTests.Auth;

public class RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? DisplayName { get; init; }
}