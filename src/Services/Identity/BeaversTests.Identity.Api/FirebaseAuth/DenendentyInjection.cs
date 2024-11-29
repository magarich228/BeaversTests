using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;

namespace BeaversTests.Identity.Api.FirebaseAuth;

public static class FirebaseExtensions
{
    private const string FirebaseConfigurationSectionName = "Firebase";
    private const string GoogleCredentialsFilePathConfigurationSectionName = "GoogleCredentialsFilePath";
    private const string GoogleAppCredentialsVariableName = "GOOGLE_APPLICATION_CREDENTIALS";
    private const string FirebaseApiKeyConfigurationSectionName = "ApiKey";
    
    public static IServiceCollection AddAuthInternal(this IServiceCollection services, IConfiguration configuration)
    {
        var firebaseProjectName = "beaverstests";

        var firebaseConfiguration = configuration.GetRequiredSection(FirebaseConfigurationSectionName);
        
        var credentialsFilePath = firebaseConfiguration.GetValue<string>(GoogleCredentialsFilePathConfigurationSectionName);
        var apiKey = firebaseConfiguration.GetValue<string>(FirebaseApiKeyConfigurationSectionName);
        
        if (!File.Exists(credentialsFilePath))
        {
            throw new IdentityException("Firebase credentials file not found");
        }
        
        Environment.SetEnvironmentVariable(GoogleAppCredentialsVariableName,
            credentialsFilePath);
        services.AddSingleton(FirebaseApp.Create());
        
        services.AddSingleton(new FirebaseAuthConfig
        {
            ApiKey = apiKey,
            AuthDomain = $"{firebaseProjectName}.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider(),
                new GoogleProvider()
            }
        });
        services.AddSingleton<FirebaseAuthClient>();
        services.AddSingleton<FirebaseObbCodeService>();
        
        services.AddSingleton<IFirebaseAuthService, FirebaseAuthService>(); 
        
        return services;
    }
}