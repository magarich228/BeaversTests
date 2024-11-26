using Firebase.Auth;
using Newtonsoft.Json.Linq;

namespace BeaversTests.Identity.Api.FirebaseAuth;

public class FirebaseAuthService(FirebaseAuthClient firebaseAuth, FirebaseObbCodeService firebaseEmail) : IFirebaseAuthService
{
    public async Task<string?> SignUp(string email, string password)
    {
        var userCredentials = await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);
        
        var emailToVerify = await firebaseEmail.SendEmailVerificationAsync(userCredentials);
        
        return emailToVerify;
    }

    public async Task<string?> Login(string email, string password)
    {
        var userCredentials = await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

        if (!userCredentials.User.Info.IsEmailVerified)
            return null;
        
        return await userCredentials.User.GetIdTokenAsync();
    }
    
    public void SignOut() => firebaseAuth.SignOut(); 
}