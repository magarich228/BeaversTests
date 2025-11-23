using System.Net.Http.Json;
using Firebase.Auth;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BeaversTests.Auth.FirebaseProvider;

internal class BeaversFirebaseService(
    ILogger<BeaversFirebaseService> logger,
    FirebaseAuthConfig config)
{
    private const string SendOobCodeRequestType = "VERIFY_EMAIL";

    public async Task<FirebaseEmailVerificationResponse> SendEmailVerificationAsync(UserCredential credential)
    {
        logger.LogTrace("Sending email verification to {Email}...", credential.User.Info.Email);

        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={config.ApiKey}";
        var jsonContent = JsonContent.Create(new
        {
            requestType = SendOobCodeRequestType,
            idToken = await credential.User.GetIdTokenAsync()
        });

        var response = await config.HttpClient.PostAsync(new Uri(url), jsonContent);
        logger.LogDebug("Firebase email verification response: {StatusCode}", response.StatusCode);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();
        logger.LogDebug("Firebase email verification response content: {ResponseContent}", responseContent);

        var emailVerificationResponse = JsonConvert.DeserializeObject<FirebaseEmailVerificationResponse>(responseContent) ??
            throw new AuthException("Failed to deserialize Firebase email verification response.");

        return emailVerificationResponse;
    }

    public async Task<FirebaseTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var url = $"https://securetoken.googleapis.com/v1/token?key={config.ApiKey}";
        var jsonContent = JsonContent.Create(new
        {
            grant_type = "refresh_token",
            refresh_token = refreshToken
        });

        var response = await config.HttpClient.PostAsync(url, jsonContent);
        logger.LogDebug("Firebase refresh token response: {StatusCode}", response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        logger.LogDebug("Firebase refresh token response content: {ResponseContent}", responseContent);

        response.EnsureSuccessStatusCode();

        var tokenResponse = JsonConvert.DeserializeObject<FirebaseTokenResponse>(responseContent) ??
            throw new AuthException("Failed to deserialize Firebase token response.");

        return tokenResponse;
    }
}