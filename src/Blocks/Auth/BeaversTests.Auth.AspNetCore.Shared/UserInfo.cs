namespace BeaversTests.Auth.AspNetCore.Shared;

public class UserInfo
{
    public required string UserId { get; init; }
    public required string Email { get; init; }
    public required string Name { get; init; }
}