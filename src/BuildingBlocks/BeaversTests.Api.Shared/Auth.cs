using System.Security.Authentication;
using BeaversTests.Common.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace BeaversTests.Api.Shared;

public static class Auth
{
    public const string AuthTokenSessionKey = "token";
    
    public static IServiceCollection AddAuth(this IServiceCollection services)
    {
        var firebaseProjectName = "beaverstests";
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"https://securetoken.google.com/{firebaseProjectName}";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{firebaseProjectName}",
                    ValidateAudience = true,
                    ValidAudience = firebaseProjectName,
                    ValidateLifetime = true
                };
            });

        services.AddScoped<IUserService, UserService>();

        return services;
    }
    
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
        
        return app;
    }

    public static string GetCurrentUserId(this HttpContext httpContext)
    {
        return httpContext.User.Claims
                   .FirstOrDefault(c => c.Type == "user_id")?.Value ??
               throw new AuthenticationException("User id not found");
    }
}