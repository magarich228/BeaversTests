using BeaversTests.Auth.Persistence;
using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.Auth.FirebaseProvider;

// TODO: Реализация консистентности состояний Firebase и БД + тесты 
internal class FirebaseUserService(
    ILogger<FirebaseUserService> logger,
    FirebaseAuth firebaseAuthAdmin,
    IAuthDbContext authDb) : IUserService
{
    public async Task<BeaversUser?> GetUserByIdAsync(string userId)
    {
        logger.LogTrace("Get user by id: {UserId}.", userId);

        var user = await authDb.Users.FindAsync(userId);

        return user;
    }

    public async Task<BeaversUser?> GetUserByEmailAsync(string email)
    {
        logger.LogTrace("Get user by email: {Email}.", email);

        var user = await authDb.Users.FirstOrDefaultAsync(u => u.Email == email);

        return user;
    }

    public async Task<UpdateUserResult> UpdateUserProfileAsync(string userId, UpdateUserProfileRequest request)
    {
        try
        {
            logger.LogTrace("Update user profile: {UserId}.", userId);
            // TODO: + подумать о структурированных логах (Serilog)

            var localUser = await authDb.Users.FindAsync(userId);
            var firebaseUserRecord = await firebaseAuthAdmin.GetUserAsync(userId);

            if (localUser == null)
            {
                logger.LogTrace("User {UserId} not found during update.", userId);
                return new UpdateUserResult()
                {
                    Error = $"User {userId} not found."
                };
            }

            localUser.DisplayName = request.DisplayName;
            localUser.PhotoUrl = request.PhotoUrl;

            authDb.Users.Update(localUser);

            if (await authDb.SaveChangesAsync() == 0)
            {
                throw new AuthException($"Failed to update user {userId}.");
            }
            
            var newUserRecord = await firebaseAuthAdmin.UpdateUserAsync(new UserRecordArgs()
            {
                Uid = firebaseUserRecord.Uid,
                DisplayName = request.DisplayName,
                PhotoUrl = request.PhotoUrl,
                Email = firebaseUserRecord.Email,
                EmailVerified = firebaseUserRecord.EmailVerified,
                PhoneNumber = firebaseUserRecord.PhoneNumber,
                Disabled = firebaseUserRecord.Disabled
            });

            return new UpdateUserResult()
            {
                Success = true
            };
        }
        catch (FirebaseAuthException ex)
        {
            logger.LogDebug("Failed to update user profile ({ErrorCode}): {ErrorMessage}.",
                ex.AuthErrorCode.ToString(), ex.Message);

            return new UpdateUserResult()
            {
                Error = $"Error code: {ex.AuthErrorCode.ToString()}\nMessage: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError("Unexpected error during user profile update: {Message}.", ex.Message);

            return new UpdateUserResult()
            {
                Error = ex.Message
            };
        }
    }
    
    public async Task<DeleteUserResult> DeleteUserAsync(string userId)
    {
        try
        {
            logger.LogTrace("Delete user: {UserId}.", userId);

            var localUser = await authDb.Users.FindAsync(userId);
            
            if (localUser == null)
            {
                return new DeleteUserResult()
                {
                    Error = $"User {userId} not found."
                };
            }

            var deletedUser = authDb.Users.Remove(localUser);

            if (await authDb.SaveChangesAsync() != 1 || deletedUser.State != EntityState.Deleted)
            {
                throw new AuthException($"Failed to delete user {userId}.");
            }
            
            await firebaseAuthAdmin.DeleteUserAsync(userId);

            return new DeleteUserResult()
            {
                Success = true
            };
        }
        catch (FirebaseAuthException ex)
        {
            logger.LogDebug("Failed to delete user ({ErrorCode}): {ErrorMessage}.",
                ex.AuthErrorCode.ToString(), ex.Message);

            return new DeleteUserResult()
            {
                Error = $"Erorr code: {ex.AuthErrorCode.ToString()}\nMessage: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError("Unexpected error during user deletion: {Message}.", ex.Message);

            return new DeleteUserResult()
            {
                Error = ex.Message
            };
        }
    }
}