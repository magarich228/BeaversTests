using BeaversTests.Auth.Persistence;
using BeaversTests.Auth.Public;
using BeaversTests.Platform.Public;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FirebaseAuthException = Firebase.Auth.FirebaseAuthException;

namespace BeaversTests.Auth.FirebaseProvider;

// TODO: Проверить FirebaseAuthClient, вероятно обеспечить stateless. Возможно расширить BeaversFirebaseService и заменить на него
internal class FirebaseAuthService(
    FirebaseAuth firebaseAuthAdmin,
    FirebaseAuthClient firebaseAuthClient,
    FirebaseClientService firebaseClientService,
    ILogger<FirebaseAuthService> logger,
    IAuthDbContext authDb) : IAuthService
{
    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        try
        {
            logger.LogTrace("{Email} log in attempt.", request.Email);

            var credential = await firebaseAuthClient.SignInWithEmailAndPasswordAsync(
                request.Email,
                request.Password);
            
            var signInResult = await firebaseClientService.SignInWithEmailAndPasswordAsync(
                request.Email,
                request.Password);

            if (!signInResult.Success)
            {
                Result.MakeFailure(signInResult.Error);
            }
            
            signInResult.Data.
            if (!credential.User.Info.IsEmailVerified)
            {
                return new AuthResult()
                {
                    UserId = credential.User.Uid,
                    Email = credential.User.Info.Email,
                    Success = false,
                    Error = "Email is not verified."
                };
            }

            await SyncUserWithLocal(credential.User);

            var response = new AuthResult()
            {
                UserId = credential.User.Uid,
                Email = credential.User.Info.Email,
                Success = true,
                RefreshToken = credential.User.Credential.RefreshToken,
                IdToken = credential.User.Credential.IdToken,
                ExpiresIn = credential.User.Credential.Created.AddSeconds(credential.User.Credential.ExpiresIn)
            };

            logger.LogTrace("User authenticated {UserId} {UserEmail}.", 
                credential.User.Uid, credential.User.Info.Email);

            firebaseAuthClient.SignOut();
            
            return response;
        }
        catch (FirebaseAuthException ex)
        {
            var response = new AuthResult()
            {
                Email = request.Email,
                Success = false,
                Error = $"Reason: {ex.Reason.ToString()}\nMessage: {ex.Message}"
            };

            logger.LogDebug("{Email} log in failed ({Reason}): {Message}",
                request.Email, ex.Reason, ex.Message);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during log in user {Email}.", request.Email);

            return new AuthResult()
            {
                Email = request.Email,
                Error = ex.Message
            };
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            logger.LogTrace("{Email} registration attempt.", request.Email);

            var credential = await firebaseAuthClient.CreateUserWithEmailAndPasswordAsync(
                request.Email,
                request.Password,
                request.DisplayName);

            // TODO:
            // await firebaseClientService.SignUpWithEmailAndPasswordAsync(
            //     request.Email,
            //     request.Password);
            
            firebaseAuthClient.SignOut();

            await CreateLocalUser(credential.User);

            var emailToVerify = await firebaseClientService.SendEmailVerificationAsync(credential);
            
            var response = new AuthResult()
            {
                UserId = credential.User.Uid,
                Email = emailToVerify.Email,
                Success = credential.User.Info.IsEmailVerified
            };

            if (credential.User.Info.IsEmailVerified)
            {
                response.Success = true;
                response.RefreshToken = credential.User.Credential.RefreshToken;
                response.IdToken = credential.User.Credential.IdToken;
                response.ExpiresIn = credential.User.Credential.Created.AddSeconds(credential.User.Credential.ExpiresIn);
                
                logger.LogWarning("{UserId} with already verified email.", credential.User.Uid);
            }
            else
            {
                response.Error = "Email is not verified.";
            }
            
            logger.LogTrace("{Email} registered succeeded.", request.Email);
            
            return response;
        }
        catch (FirebaseAuthException ex)
        {
            var response = new AuthResult()
            {
                Email = request.Email,
                Success = false,
                Error = $"Reason: {ex.Reason.ToString()}\nMessage: {ex.Message}"
            };

            logger.LogDebug("{Email} log in failed: {Message}", request.Email, ex.Message);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during registration with email {Email}.", request.Email);
            
            return new AuthResult()
            {
                Email = request.Email,
                Error = ex.Message
            };
        }
    }

    public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            logger.LogTrace("Refresh token attempt.");

            var tokenResponse = await firebaseClientService.RefreshTokenAsync(request.RefreshToken);

            var userRecord = await firebaseAuthAdmin.GetUserAsync(tokenResponse.UserId);

            await SyncUserAfterRefresh(userRecord);

            var authResponse = new AuthResult()
            {
                UserId = tokenResponse.UserId,
                Email = userRecord.Email,
                Success = true,
                RefreshToken = tokenResponse.RefreshToken,
                IdToken = tokenResponse.IdToken,
                ExpiresIn = DateTime.UtcNow.AddSeconds(int.Parse(tokenResponse.ExpiresIn))
            };

            logger.LogTrace("Token refreshed for user {UserId}.", tokenResponse.UserId);

            return authResponse;
        }
        catch (FirebaseAdmin.Auth.FirebaseAuthException ex)
        {
            var response = new AuthResult()
            {
                Success = false,
                Error = $"Error code: {ex.ErrorCode.ToString()}\nMessage: {ex.Message}"
            };

            logger.LogDebug("Refresh token failed: {Message}", ex.Message);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during token refresh.");
            
            return new AuthResult()
            {
                Error = ex.Message
            };
        }
    }

    public async Task<LogoutResult> LogoutAsync(string userId)
    {
        try
        {
            logger.LogTrace("User {UserId} log out attempt.", userId);

            if (firebaseAuthClient.User != null)
            {
                firebaseAuthClient.SignOut();
            }

            await firebaseAuthAdmin.RevokeRefreshTokensAsync(userId);

            logger.LogTrace("User {UserId} log out succeeded.", userId);

            return new LogoutResult
            {
                Success = true
            };
        }
        catch (FirebaseAdmin.Auth.FirebaseAuthException ex)
        {
            logger.LogError("User {UserId} log out failed ({ErrorCode}): {ErrorMessage}.",
                userId, ex.ErrorCode.ToString(), ex.Message);

            return new LogoutResult()
            {
                Error = $"Error code: {ex.ErrorCode.ToString()}\nMessage: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError("Unexpected error during user log out: {Message}.", ex.Message);

            return new LogoutResult()
            {
                Error = ex.Message
            };
        }
    }

    public Task ChangeEmailAsync()
    {
        throw new NotImplementedException();
    }
    
    public Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        throw new NotImplementedException();
    }

    private async Task SyncUserWithLocal(User firebaseUser)
    {
        logger.LogTrace("Login synchronizing user {UserId} {Email}.", firebaseUser.Uid, firebaseUser.Info?.Email);
        
        var user = await authDb.Users
            .FirstOrDefaultAsync(u => u.Id == firebaseUser.Uid);

        if (user == null)
        {
            throw new AuthException($"User not found in database {firebaseUser.Uid} {firebaseUser.Info?.Email}.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        user.EmailVerified = firebaseUser.Info?.IsEmailVerified ?? user.EmailVerified;

        logger.LogTrace("User {UserId} {Email} updated.", firebaseUser.Uid, firebaseUser.Info?.Email);
        
        if (await authDb.SaveChangesAsync() == 0)
        {
            throw new AuthException($"Failed to update user {firebaseUser.Uid}.");
        }
    }

    private async Task CreateLocalUser(User firebaseUser)
    {
        logger.LogTrace("Creating local user {UserId} {Email}.", firebaseUser.Uid, firebaseUser.Info?.Email);
        // TODO: Password
        var user = new BeaversUser()
        {
            Id = firebaseUser.Uid,
            Email = firebaseUser.Info?.Email ?? string.Empty,
            DisplayName = firebaseUser.Info?.DisplayName,
            EmailVerified = firebaseUser.Info?.IsEmailVerified ?? false,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        authDb.Users.Add(user);

        if (await authDb.SaveChangesAsync() == 0)
        {
            throw new AuthException("Failed to create user.");
        }
        
        logger.LogTrace("Local user {UserId} {Email} created.",
            firebaseUser.Uid, firebaseUser.Info?.Email);
    }
    
    private async Task SyncUserAfterRefresh(UserRecord userRecord)
    {
        logger.LogTrace("Sync user during token refresh {UserId}.", userRecord.Uid);
        
        var user = await authDb.Users
            .FirstOrDefaultAsync(u => u.Id == userRecord.Uid);

        if (user == null)
        {
            logger.LogError("User {UserId} not found in database during token refresh.", userRecord.Uid);
            return;
        }

        user.LastLoginAt = userRecord.UserMetaData.LastRefreshTimestamp;
        user.EmailVerified = userRecord.EmailVerified;

        if (await authDb.SaveChangesAsync() == 0)
        {
            logger.LogError("Failed to update user {UserId} during token refresh.", userRecord.Uid);
        }
        
        logger.LogTrace("User {UserId} updated during token refresh.", userRecord.Uid);
    }
}