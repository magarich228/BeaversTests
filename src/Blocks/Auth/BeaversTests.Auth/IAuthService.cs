namespace BeaversTests.Auth;
// TODO: Cancellation Tokens
// TODO: validation
public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request);
    Task<LogoutResult> LogoutAsync(string userId);
    Task ChangeEmailAsync();
    Task ResetPasswordAsync(ResetPasswordRequest request);
}