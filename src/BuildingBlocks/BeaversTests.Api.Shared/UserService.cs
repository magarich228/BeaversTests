using BeaversTests.Common.Application;
using Microsoft.AspNetCore.Http;

namespace BeaversTests.Api.Shared;

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    public string GetCurrentUserId()
    {
        var httpContext = httpContextAccessor.HttpContext ??
                          throw new Exception("Http context not found.");

        return httpContext.GetCurrentUserId();
    }
}