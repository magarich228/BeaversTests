using BeaversTests.Auth.Public;
using Microsoft.AspNetCore.Http;

namespace BeaversTests.Auth.AspNetCore.Shared;

public class UserContext(IHttpContextAccessor httpContextAccessor)
{
    public string GetCurrentUserId()
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception("Http context not found.");

        return httpContext.GetCurrentUserId();
    }

    public UserInfo GetCurrentUserInfo()
    {
        var httpContext = httpContextAccessor.HttpContext ??
            throw new Exception("Http context not found.");

        return httpContext.User.GetCurrentUserInfo();
    }
}