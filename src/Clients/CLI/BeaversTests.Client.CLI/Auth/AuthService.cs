using System.Text;
using BeaversTests.Auth.Public;
using BeaversTests.Client.Http;
using Newtonsoft.Json;

namespace BeaversTests.Client.CLI.Auth;

// ReSharper disable once ClassNeverInstantiated.Global
internal class AuthService(
    IBeaversTestsAuthClient authClient,
    ProfileManager profileManager)
{
    private readonly IBeaversTestsAuthClient _authClient = authClient ??
                                                           throw new BeaversTestsCliException(
                                                               $"Failed to resolve {nameof(IBeaversTestsAuthClient)}");
    private readonly ProfileManager _profileManager = profileManager ??
                                                      throw new BeaversTestsCliException(
                                                          $"Failed to resolve {nameof(ProfileManager)}");

    public const string CredentialsFileEnvironmentVariable = "BEAVERS_CREDENTIALS_FILE";
    public const string BeaversUserEmailEnvironmentVariable = "BEAVERS_USER_EMAIL";
    public const string BeaversUserPasswordEnvironmentVariable = "BEAVERS_USER_PASSWORD";
    
    public AuthResult Login(string? credentialsFilePath, CancellationToken cancellationToken = default)
    {
        credentialsFilePath ??= Environment.GetEnvironmentVariable(CredentialsFileEnvironmentVariable);

        var userEmail = Environment.GetEnvironmentVariable(BeaversUserEmailEnvironmentVariable);
        var userPassword =
            Environment.GetEnvironmentVariable(BeaversUserPasswordEnvironmentVariable);

        if (!TryGetLoginModel(
                credentialsFilePath,
                userEmail,
                userPassword,
                out var loginModel,
                out var errorMessage,
                out var exitCode))
        {
            return new AuthResult()
            {
                Success = false,
                Error = $"{errorMessage} ({exitCode})"
            };
        }

        var authResult = _authClient.LoginAsync(loginModel!, cancellationToken)
            .GetAwaiter()
            .GetResult();

        return ValidateAndGetResult(authResult, credentialsFilePath);
    }

    public AuthResult RefreshOrLogin(CancellationToken cancellationToken = default)
    {
        var credentialsProfile = _profileManager.LoadCredentialsProfile();

        if (credentialsProfile.IsExpired)
        {
            return Login(credentialsProfile.CredentialsFilePath, cancellationToken);
        }

        if (credentialsProfile.ShouldRefresh)
        {
            var request = new RefreshTokenRequest(credentialsProfile.RefreshToken ?? 
                                                  throw new BeaversTestsCliException("Failed to get refresh token"));
            
            var authResult = _authClient.RefreshTokenAsync(request, cancellationToken)
                .GetAwaiter()
                .GetResult();

            return ValidateAndGetResult(authResult, credentialsProfile.CredentialsFilePath);
        }

        _authClient.SetAccessToken(credentialsProfile.AccessToken ??
                                   throw new BeaversTestsCliException("Failed to get access token from profile"));
        
        return new AuthResult()
        {
            Success = true,
            Error = null
        };
    }

    public UserInfoResult GetCurrentUser(CancellationToken cancellationToken = default)
    {
        var verificationResult = _authClient.GetCurrentUserAsync(cancellationToken)
            .GetAwaiter()
            .GetResult();

        if (!verificationResult.Success)
        {
            return new UserInfoResult()
            {
                Success = verificationResult.Success,
                Error = $"Failed to login ({verificationResult.StatusCode}): {verificationResult.Error}"
            };
        }

        if (!verificationResult.Data?.IsAuthenticated ??
            throw new BeaversTestsCliException(
                $"Failed to current user info. {nameof(verificationResult.Data)} is null"))
        {
            return new UserInfoResult()
            {
                Success = false,
                Error = $"Not authenticated"
            };
        }

        return new UserInfoResult()
        {
            Success = true,
            UserInfo = verificationResult.Data?.UserInfo ?? throw new BeaversTestsCliException("Failed to get user info"),
            Error = null
        };
    }
    
    private bool TryGetLoginModel(
        string? credentialsFilePath,
        string? userEmail,
        string? userPassword,
        out LoginRequest? loginModel,
        out string? errorMessage,
        out int? exitCode)
    {
        if (credentialsFilePath is not null)
        {
            using var credentialsFile =
                File.Open(credentialsFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            using var streamReader = new StreamReader(credentialsFile, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(streamReader);
            var serializer = JsonSerializer.CreateDefault();

            loginModel = serializer.Deserialize<LoginRequest>(jsonReader);

            if (loginModel is null)
            {
                errorMessage = "Credentials deserialization failed.";
                exitCode = 1;

                return false;
            }
        }
        else
        {
            if (userEmail is null || userPassword is null)
            {
                exitCode = 1;
                errorMessage = "Credentials env variables are not set.";
                loginModel = null;

                return false;
            }

            loginModel = new LoginRequest(userEmail, userPassword);
        }

        errorMessage = null;
        exitCode = null;

        return true;
    }

    private AuthResult ValidateAndGetResult(
        BeaversApiResponse<BeaversTests.Auth.Public.AuthResult> authResult, 
        string? credentialsFilePath)
    {
        ArgumentNullException.ThrowIfNull(authResult);
        
        if (!authResult.Success)
        {
            return new AuthResult()
            {
                Success = authResult.Success,
                Error = $"Failed to login ({authResult.StatusCode}): {authResult.Error}"
            };
        }

        if (!authResult.Data?.Success ?? throw new BeaversTestsCliException($"Failed to get login result. {nameof(authResult.Data)} is null"))
        {
            return new AuthResult()
            {
                Success = false,
                Error = $"Failed to login: {authResult.Data?.Error}"
            };
        }
        
        _profileManager.SaveTokens(authResult.Data!, credentialsFilePath);
        _authClient.SetAccessToken(authResult.Data?.IdToken ?? 
                                   throw new BeaversTestsCliException("Failed to get access token."));

        return new AuthResult()
        {
            Success = true,
            Error = null
        };
    }

    internal class AuthResult // TODO: На уровне всей платформы реализовать Result pattern
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    internal class UserInfoResult : AuthResult
    {
        public UserInfo? UserInfo { get; set; }
    }
}