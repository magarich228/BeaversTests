using Firebase.Auth;
using Firebase.Auth.Providers;
using FirebaseAdmin;

namespace BeaversTests.Identity.Api.FirebaseAuth;

public static class FirebaseExtensions
{
    public static IServiceCollection AddAuthInternal(this IServiceCollection services)
    {
        var firebaseProjectName = "beaverstests";
        
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS",
            "beaverstests-firebase-adminsdk-kp03n-8921020f2f.json");
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