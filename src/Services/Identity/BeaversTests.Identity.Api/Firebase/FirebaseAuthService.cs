using Firebase.Auth;

namespace BeaversTests.Identity.Api.Firebase;

public class FirebaseAuthService(FirebaseAuthClient firebaseAuth) : IFirebaseAuthService
{
    public async Task<string?> SignUp(string email, string password)
    {
        var userCredentials = await firebaseAuth.CreateUserWithEmailAndPasswordAsync(email, password);

        return userCredentials is null ? null : await userCredentials.User.GetIdTokenAsync();
    }

    public async Task<string?> Login(string email, string password)
    {
        var userCredentials = await firebaseAuth.SignInWithEmailAndPasswordAsync(email, password);

        return userCredentials is null ? null : await userCredentials.User.GetIdTokenAsync();
    }
    
    public void SignOut() => firebaseAuth.SignOut(); 
}