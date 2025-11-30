using System.Threading;
using System.Threading.Tasks;
using BeaversTests.Auth.Public;
using BeaversTests.Client.Http.Configuration;

namespace BeaversTests.Client.Http;

public interface IBeaversTestsAuthClient
{
    Task<BeaversApiResponse<AuthResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<BeaversApiResponse<AuthResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<BeaversApiResponse<AuthResult>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<BeaversApiResponse<LogoutResult>> LogoutAsync(CancellationToken cancellationToken = default);
    Task<BeaversApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<BeaversApiResponse<VerificationResult>> GetCurrentUserAsync(CancellationToken cancellationToken = default);
        
    // Token management
    void SetAccessToken(string accessToken);
    void ClearTokens();
    AuthTokens? GetCurrentTokens();
}