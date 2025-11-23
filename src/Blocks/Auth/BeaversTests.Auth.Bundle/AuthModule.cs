using BeaversTests.Auth.FirebaseProvider;
using BeaversTests.Platform.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.Auth.Bundle;

public static class AuthModule
{
    public static IServiceCollection AddAuthModule(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var authConfigurationSection = configuration.GetRequiredSection(AuthConfigurationConstants.AuthConfigurationSectionName);
        var authProvider = authConfigurationSection[AuthConfigurationConstants.AuthProviderSection];

        switch (authProvider)
        {
            case AuthConfigurationConstants.FirebaseAuthProviderName:
                services.AddFirebaseAuthProvider(configuration);
                break;
            
            default:
                throw new AuthException($"Auth provider not found: {authProvider}");
        }

        return services;
    }
}