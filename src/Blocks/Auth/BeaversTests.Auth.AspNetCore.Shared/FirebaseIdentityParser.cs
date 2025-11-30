using Newtonsoft.Json;

namespace BeaversTests.Auth.AspNetCore.Shared;

// TODO: Лучше вынести в отдельную сборку провайдера Firebase
internal static class FirebaseIdentityParser
{
    public const string FirebaseIdentityKey = "firebase";
    
    public static FirebaseIdentity? Parse(string jsonIdentity)
    {
        var identity = JsonConvert.DeserializeObject<FirebaseIdentity>(jsonIdentity);

        return identity;
    }
}