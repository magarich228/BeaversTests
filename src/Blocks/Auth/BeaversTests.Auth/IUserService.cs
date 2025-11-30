using BeaversTests.Auth.Public;

namespace BeaversTests.Auth;

public interface IUserService
{
    Task<BeaversUser?> GetUserByIdAsync(string userId);
    Task<BeaversUser?> GetUserByEmailAsync(string email);
    Task<UpdateUserResult> UpdateUserProfileAsync(string userId, UpdateUserProfileRequest request);
    Task<DeleteUserResult> DeleteUserAsync(string userId);
}