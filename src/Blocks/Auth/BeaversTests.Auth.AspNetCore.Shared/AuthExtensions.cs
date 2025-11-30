using System.Security.Authentication;
using BeaversTests.Platform;
using BeaversTests.Platform.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BeaversTests.Auth.AspNetCore.Shared;

public static class AuthExtensions
{
    public static IServiceCollection AddBeaversTestsAuth(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var authConfigurationSection = configuration.GetRequiredSection(AuthConfigurationConstants.AuthConfigurationSectionName);
        var authProvider = authConfigurationSection[AuthConfigurationConstants.AuthProviderSection];

        var firebaseConfigurationSection = authConfigurationSection.GetRequiredSection(AuthConfigurationConstants.FirebaseAuthProviderName);
        
        var authProviderInfo = authProvider switch
        {
            AuthConfigurationConstants.FirebaseAuthProviderName => new AuthProviderInfo()
            {
                Audience = firebaseConfigurationSection["Audience"] ?? throw new BeaversTestsInternalException("Firebase audience not found"),
                Authority = firebaseConfigurationSection["Authority"] ?? throw new BeaversTestsInternalException("Firebase authority not found"),
                Issuer = firebaseConfigurationSection["Issuer"] ?? throw new BeaversTestsInternalException("Firebase issuer not found")
            },
            _ => throw new BeaversTestsInternalException($"Auth provider configuration section not found: {authProvider}")
        };
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authProviderInfo.Authority;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authProviderInfo.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authProviderInfo.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    RequireExpirationTime = true
                };

                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger(nameof(AuthExtensions));
                        logger.LogDebug(context.Exception, "Authentication failed");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
                        var logger = loggerFactory.CreateLogger(nameof(AuthExtensions));
                        logger.LogTrace("Token validated for user: {UserId}", context.Principal?.GetCurrentUserInfo() ??
                                                                              throw new AuthenticationException("Token validated, but user not found"));
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddScoped<UserContext>();

        return services;
    }

    public static IApplicationBuilder UseBeaversTestsAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}