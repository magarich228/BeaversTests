using System.Security.Authentication;
using System.Security.Claims;
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
        services.AddSession();
        services.AddDistributedMemoryCache();
        
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

        return services;
    }
    
    public static IApplicationBuilder UseAuth(this IApplicationBuilder app)
    {
        app.UseSession();
        
        app.Use(async (context, next) =>
        {
            var token = context.Session.GetString(AuthTokenSessionKey);
    
            if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Append("Authorization", "Bearer " + token);
            }

            await next();
        });
        
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