using System.Security.Cryptography;

namespace BeaversTests.TestRunnerController.Core;

public class ControllerKeyDomainService
{
    public string GenerateConnectionKey()
    {
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();

        byte[] data = new byte[32];
        rng.GetBytes(data);
        
        return Convert.ToBase64String(data);
    }
}