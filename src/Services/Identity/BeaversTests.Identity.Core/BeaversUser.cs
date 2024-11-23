namespace BeaversTests.Identity.Core;

public class BeaversUser
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public bool EmailConfirmed { get; init; }
    public required string PasswordHash { get; init; }
}