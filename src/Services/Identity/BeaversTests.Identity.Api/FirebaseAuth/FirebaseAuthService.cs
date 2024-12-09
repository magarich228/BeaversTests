using Firebase.Auth;
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
        
        // TODO: send refresh token.
        return await userCredentials.User.GetIdTokenAsync();
    }
    
    // TODO: password reset endpoint
    // TODO: email update endpoint
    // TODO: refresh token endpoint
    
    public void SignOut() => firebaseAuth.SignOut(); 
}