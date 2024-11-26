using Firebase.Auth;
using Newtonsoft.Json.Linq;

namespace BeaversTests.Identity.Api.FirebaseAuth;

public class FirebaseObbCodeService(FirebaseAuthConfig config)
{
    private const string SendOobCodeRequestType = "VERIFY_EMAIL";
    
    public async Task<string> SendEmailVerificationAsync(UserCredential credential)
    {
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={config.ApiKey}";
        var jsonContent = JsonContent.Create(new
        {
            requestType = SendOobCodeRequestType,
            idToken = await credential.User.GetIdTokenAsync()
        });

        var response = await config.HttpClient.PostAsync(new Uri(url), jsonContent);

        response.EnsureSuccessStatusCode();
        
        return JToken.Parse(await response.Content.ReadAsStringAsync())["email"]?.ToString() ??
               throw new IdentityException("Failed to parse email verification response.");
    }
}