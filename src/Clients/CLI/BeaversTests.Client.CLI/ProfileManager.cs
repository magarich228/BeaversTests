using System.Diagnostics.CodeAnalysis;
using System.Text;
using BeaversTests.Auth.Public;
using Newtonsoft.Json;

namespace BeaversTests.Client.CLI;


internal class ProfileManager
{
    private readonly string _configDir;
    private readonly string _profileFile;
    
    public ProfileManager()
    {
        // ~/.beavers/credentialsProfile (Linux/macOS) или %USERPROFILE%\.beavers\credentialsProfile (Windows)
        _configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".beavers"
        );
        _profileFile = Path.Combine(_configDir, "credentialsProfile");
    }
    
    public void SaveTokens(AuthResult auth, string? credentialsFilePath = null)
    {
        if (!Directory.Exists(_configDir))
            Directory.CreateDirectory(_configDir);
            
        var tokenData = new ProfileCredentialsData
        {
            UserId = auth.UserId,
            Email = auth.Email,
            AccessToken = auth.IdToken,
            RefreshToken = auth.RefreshToken,
            ExpiresAt = auth.ExpiresIn ?? throw new BeaversTestsCliException("Failed to get expiration date"),
            IssuedAt = DateTime.UtcNow
        };
        
        var json = JsonConvert.SerializeObject(tokenData, Formatting.Indented);
        var encryptedJson = Base64Encrypt(json);
        
        File.WriteAllText(_profileFile, encryptedJson);
        
        SetSecureFilePermissions(_profileFile);
    }
    
    public ProfileCredentialsData LoadCredentialsProfile()
    {
        if (!File.Exists(_profileFile))
            throw new BeaversTestsCliException("Profile file not found");
            
        var encryptedJson = File.ReadAllText(_profileFile);
        var json = SimpleDecrypt(encryptedJson);
        
        return JsonConvert.DeserializeObject<ProfileCredentialsData>(json) ?? 
               throw new BeaversTestsCliException("Failed to deserialize profile data");
    }
    
    [SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы")]
    private void SetSecureFilePermissions(string filePath)
    {
        if (Environment.OSVersion.Platform == PlatformID.Unix || 
            Environment.OSVersion.Platform == PlatformID.MacOSX)
        {
            File.SetUnixFileMode(filePath, 
                UnixFileMode.UserRead | UnixFileMode.UserWrite);
        }
    }
    
    private string Base64Encrypt(string plainText)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
    }
    
    private string SimpleDecrypt(string encryptedText)
    {
        var bytes = Convert.FromBase64String(encryptedText);
        return Encoding.UTF8.GetString(bytes);
    }
}