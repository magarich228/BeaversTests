using System.Net.Http.Json;
using BeaversTests.Platform.Public;
using Firebase.Auth;
using Microsoft.Extensions.Logging;

namespace BeaversTests.Auth.FirebaseProvider;

// TODO: Add Polly
internal class FirebaseClientService(
    ILogger<FirebaseClientService> logger,
    FirebaseAuthConfig config)
{
    private const string SendOobCodeRequestType = "VERIFY_EMAIL";
    private const string IdentityToolkitBaseUrl = "https://identitytoolkit.googleapis.com";
    private const string SecureTokenBaseUrl = "https://securetoken.googleapis.com";

    public async Task<Result<FirebaseSignInResponse>> SignInWithEmailAndPasswordAsync(string email, string password)
    {
        logger.LogTrace("Signing in {Email}...", email);
        
        var url = $"{IdentityToolkitBaseUrl}/v1/accounts:signInWithPassword?key={config.ApiKey}";
        var jsonContent = JsonContent.Create(new
        {
            email,
            password,
            returnSecureToken = true
        });
        
        var response = await config.HttpClient.PostAsync(new Uri(url), jsonContent);
        logger.LogDebug("Firebase sign in response: {StatusCode}", response.StatusCode);

        if (!response.IsSuccessStatusCode)
        {
            Result.MakeFailure(await response.Content.ReadAsStringAsync());
        }

        var signInResponse = await response.Content.ReadFromJsonAsync<FirebaseSignInResponse>() ??
                             throw new AuthException("Failed to deserialize Firebase sign in response.");
        
        logger.LogTrace("Signed in {Email}.", email);

        if (signInResponse.Registered)
        {
            Result.MakeFailure();
        }
        
        return Result<FirebaseSignInResponse>.MakeSuccess(signInResponse);
    }

    public async Task<FirebaseSignUpResponse> SignUpWithEmailAndPasswordAsync(string email, string password)
    {
        logger.LogTrace("Signing up {Email}...", email);
        
        var url = $"{IdentityToolkitBaseUrl}/v1/accounts:signUp?key={config.ApiKey}";
        var jsonContent = JsonContent.Create(new
        {
            email,
            password,
            returnSecureToken = true
        });
        
        var response = await config.HttpClient.PostAsync(new Uri(url), jsonContent);
        logger.LogDebug("Firebase sign up response: {StatusCode}", response.StatusCode);
        
        response.EnsureSuccessStatusCode();

        var signUpResponse = await response.Content.ReadFromJsonAsync<FirebaseSignUpResponse>() ??
                             throw new AuthException("Failed to deserialize Firebase sign up response.");
        
        logger.LogTrace("Signed up {Email}.", email);
        
        return signUpResponse;
    }
    
    public async Task<FirebaseEmailVerificationResponse> SendEmailVerificationAsync(UserCredential credential)
    {
        logger.LogTrace("Sending email verification to {Email}...", credential.User.Info.Email);

        var url = $"{IdentityToolkitBaseUrl}/v1/accounts:sendOobCode?key={config.ApiKey}";
        var jsonContent = JsonContent.Create(new
        {
            requestType = SendOobCodeRequestType,
            idToken = await credential.User.GetIdTokenAsync()
        });

        var response = await config.HttpClient.PostAsync(new Uri(url), jsonContent);
        logger.LogDebug("Firebase email verification response: {StatusCode}", response.StatusCode);

        response.EnsureSuccessStatusCode();

        var emailVerificationResponse = await response.Content.ReadFromJsonAsync<FirebaseEmailVerificationResponse>() ??
            throw new AuthException("Failed to deserialize Firebase email verification response.");
        
        logger.LogTrace("Sent email verification to {Email}.", credential.User.Info.Email);

        return emailVerificationResponse;
    }

    public async Task<FirebaseTokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var url = $"{SecureTokenBaseUrl}/v1/token?key={config.ApiKey}";
        var jsonContent = JsonContent.Create(new
        {
            grant_type = "refresh_token",
            refresh_token = refreshToken
        });

        var response = await config.HttpClient.PostAsync(url, jsonContent);
        logger.LogDebug("Firebase refresh token response: {StatusCode}", response.StatusCode);

        response.EnsureSuccessStatusCode();

        var tokenResponse = await response.Content.ReadFromJsonAsync<FirebaseTokenResponse>() ??
            throw new AuthException("Failed to deserialize Firebase token response.");

        return tokenResponse;
    }
}