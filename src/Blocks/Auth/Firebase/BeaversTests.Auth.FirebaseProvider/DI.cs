using BeaversTests.Platform.Configuration;
using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.Auth.FirebaseProvider;

internal static class DI
{
    private const string FirebaseConfigurationSectionName = "Firebase";
    private const string GoogleCredentialsFilePathConfigurationSectionName = "GoogleCredentialsFilePath";
    private const string GoogleAppCredentialsVariableName = "GOOGLE_APPLICATION_CREDENTIALS";
    private const string FirebaseApiKeyConfigurationSectionName = "ApiKey";
    
    private const string FirebaseProjectNameDefault = "beaverstests";
    
    public static IServiceCollection AddFirebaseAuthProvider(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddFirebaseInternal(configuration);
        
        services.AddScoped<BeaversFirebaseService>();
        
        services.AddScoped<IAuthService, FirebaseAuthService>();
        services.AddScoped<IUserService, FirebaseUserService>();

        return services;
    }

    private static IServiceCollection AddFirebaseInternal(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var authConfigurationSection =
            configuration.GetRequiredSection(AuthConfigurationConstants.AuthConfigurationSectionName);
        var firebaseConfiguration = authConfigurationSection.GetRequiredSection(AuthConfigurationConstants.FirebaseAuthProviderName);
        
        var credentialsFilePath = firebaseConfiguration[GoogleCredentialsFilePathConfigurationSectionName];
        var apiKey = firebaseConfiguration[FirebaseApiKeyConfigurationSectionName];
        
        if (!File.Exists(credentialsFilePath))
        {
            throw new AuthException($"Firebase credentials file not found: {credentialsFilePath}");
        }
        
        Environment.SetEnvironmentVariable(GoogleAppCredentialsVariableName,
            credentialsFilePath);
        
        var firebaseApp = FirebaseApp.Create();
        var firebaseAuth = FirebaseAuth.GetAuth(firebaseApp);
        
        var firebaseProjectName = (firebaseApp.Options?.Credential?.UnderlyingCredential as ServiceAccountCredential)
            ?.ProjectId ?? FirebaseProjectNameDefault;
        
        services.AddSingleton(firebaseApp);
        services.AddScoped<FirebaseAuthClient>();
        services.AddSingleton(firebaseAuth);
        
        services.AddSingleton(new FirebaseAuthConfig
        {
            ApiKey = apiKey,
            AuthDomain = $"{firebaseProjectName}.firebaseapp.com",
            Providers =
            [
                new EmailProvider(),
                new GoogleProvider()
            ]
        });

        return services;
    }
}