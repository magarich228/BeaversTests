using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BeaversTests.Auth.Public;
using BeaversTests.Client.Http.Configuration;
using Microsoft.Extensions.Logging;

namespace BeaversTests.Client.Http;

public class BeaversTestsAuthClient(
    HttpClient httpClient,
    BeaversTestsClientOptions options,
    ILogger<BeaversTestsAuthClient> logger)
    : BeaversTestsBaseClient(httpClient, options, logger), IBeaversTestsAuthClient
{
    public async Task<BeaversApiResponse<AuthResult>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email)) 
            throw new ArgumentException("Email is required", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Password)) 
            throw new ArgumentException("Password is required", nameof(request));

        var response = await SendAsync<AuthResult>(
            HttpMethod.Post,
            "api/v1/auth/login",
            request,
            cancellationToken).ConfigureAwait(false);

        // Update tokens if login was successful
        if (response.Success && response.Data?.Success == true)
        {
            AuthTokens = new AuthTokens
            {
                AccessToken = response.Data.IdToken,
                RefreshToken = response.Data.RefreshToken,
                ExpiresAt = response.Data.ExpiresIn
            };
            
            if (!string.IsNullOrEmpty(response.Data.IdToken))
            {
                SetAccessToken(response.Data.IdToken);
            }
        }

        return response;
    }

    public async Task<BeaversApiResponse<AuthResult>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email)) 
            throw new ArgumentException("Email is required", nameof(request));
        if (string.IsNullOrWhiteSpace(request.Password)) 
            throw new ArgumentException("Password is required", nameof(request));

        var response = await SendAsync<AuthResult>(
            HttpMethod.Post,
            "api/v1/auth/register",
            request,
            cancellationToken).ConfigureAwait(false);

        // Update tokens if registration was successful and user is automatically logged in
        if (response.Success && response.Data?.Success == true && !string.IsNullOrEmpty(response.Data.IdToken))
        {
            AuthTokens = new AuthTokens
            {
                AccessToken = response.Data.IdToken,
                RefreshToken = response.Data.RefreshToken,
                ExpiresAt = response.Data.ExpiresIn
            };
            SetAccessToken(response.Data.IdToken);
        }

        return response;
    }

    public async Task<BeaversApiResponse<AuthResult>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.RefreshToken)) 
            throw new ArgumentException("Refresh token is required", nameof(request));

        var response = await SendAsync<AuthResult>(
            HttpMethod.Post,
            "api/v1/auth/refresh-token",
            request,
            cancellationToken).ConfigureAwait(false);

        // Update tokens if refresh was successful
        if (response.Success && response.Data?.Success == true)
        {
            AuthTokens = new AuthTokens
            {
                AccessToken = response.Data.IdToken,
                RefreshToken = response.Data.RefreshToken,
                ExpiresAt = response.Data.ExpiresIn
            };
            
            if (!string.IsNullOrEmpty(response.Data.IdToken))
            {
                SetAccessToken(response.Data.IdToken);
            }
        }

        return response;
    }

    public async Task<BeaversApiResponse<LogoutResult>> LogoutAsync(CancellationToken cancellationToken = default)
    {
        var response = await SendAsync<LogoutResult>(
            HttpMethod.Post,
            "api/v1/auth/logout",
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Clear tokens on logout
        if (response.Success)
        {
            ClearTokens();
        }

        return response;
    }

    public async Task<BeaversApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) 
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrWhiteSpace(request.Email)) 
            throw new ArgumentException("Email is required", nameof(request));

        return await SendAsync<object>(
            HttpMethod.Post,
            "api/v1/auth/reset-password",
            request,
            cancellationToken).ConfigureAwait(false);
    }

    public async Task<BeaversApiResponse<VerificationResult>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        return await SendAsync<VerificationResult>(
            HttpMethod.Get,
            "api/v1/auth/me",
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public void ClearTokens()
    {
        AuthTokens = null;
        ClearAccessToken();
    }

    public AuthTokens? GetCurrentTokens()
    {
        return AuthTokens;
    }
}