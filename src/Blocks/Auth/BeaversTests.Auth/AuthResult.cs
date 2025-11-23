namespace BeaversTests.Auth;

public class AuthResult : OperationResult
{
    public string? IdToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? UserId { get; init; }
    public string? Email { get; init; }
    public DateTime? ExpiresIn { get; set; }
}