using System.Security.Authentication;
using System.Security.Claims;
using BeaversTests.Auth.Public;
using Microsoft.AspNetCore.Http;

namespace BeaversTests.Auth.AspNetCore.Shared;

public static class HttpContextExtensions
{
    public const string UserIdKey = "user_id";
    public const string EmailKey = "email";
    public const string DisplayNameKey = "name";

    public static string GetCurrentUserId(this HttpContext httpContext)
    {
        return httpContext.User.Claims
                   .FirstOrDefault(c => c.Type == "user_id")?.Value ??
               throw new AuthenticationException("User id not found");
    }

    public static UserInfo GetCurrentUserInfo(this ClaimsPrincipal claimsPrincipal)
    {
        // TODO: В будущем учесть то, что например Email может сохраняться по разному в зависимости от провайдера
        var firebaseIdentity = FirebaseIdentityParser.Parse(
            claimsPrincipal.FindFirst(FirebaseIdentityParser.FirebaseIdentityKey)?.Value ??
            throw new AuthenticationException("Firebase identity not found"));

        return new UserInfo(
            claimsPrincipal.FindFirst(UserIdKey)?.Value ??
            throw new AuthenticationException("User id not found"),
            claimsPrincipal.FindFirst(DisplayNameKey)?.Value ??
            throw new AuthenticationException("Name not found"),
            firebaseIdentity?.GetFirstIdentityByKey(EmailKey) ??
            throw new AuthenticationException("Email not found"));
    }
}