using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;

namespace BeaversTests.Identity.Api.FirebaseAuth;

public static class FirebaseExtensions
{
    private const string GoogleAppCredentialsVariableName = "GOOGLE_APPLICATION_CREDENTIALS";
    private const string CredentialsFileName = "beaverstests-firebase-adminsdk-kp03n-8921020f2f.json";
    
    public static IServiceCollection AddAuthInternal(this IServiceCollection services)
    {
        var firebaseProjectName = "beaverstests";

        if (!File.Exists(CredentialsFileName))
        {
            throw new IdentityException("Firebase credentials file not found");
        }
        
        Environment.SetEnvironmentVariable(GoogleAppCredentialsVariableName,
            CredentialsFileName);
        services.AddSingleton(FirebaseApp.Create());
        
        services.AddSingleton(new FirebaseAuthConfig
        {
            ApiKey = "AIzaSyDh58sllS39Z6RSE-LTheJ_Adgbrl2Ot1c",
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