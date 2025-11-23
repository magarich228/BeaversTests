namespace BeaversTests.Auth;

public class BeaversUser
{
    public required string Id { get; init; }
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public string? PhotoUrl { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; set; }
    public string? PhoneNumber { get; set; }
}