using System.Diagnostics.CodeAnalysis;
using BeaversTests.Auth.Public;

namespace BeaversTests.Client.CLI;

using System.Text;
using Newtonsoft.Json;

internal class ProfileManager
{
    private readonly string _configDir;
    private readonly string _profileFile;
    
    public ProfileManager()
    {
        // ~/.beavers/config (Linux/macOS) или %USERPROFILE%\.beavers\config (Windows)
        _configDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".beavers"
        );
        _profileFile = Path.Combine(_configDir, "profile");
    }
    
    public void SaveTokens(AuthResult auth)
    {
        if (!Directory.Exists(_configDir))
            Directory.CreateDirectory(_configDir);
            
        var tokenData = new ProfileData
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
    
    public ProfileData LoadProfile()
    {
        if (!File.Exists(_profileFile))
            throw new BeaversTestsCliException("Profile file not found");
            
        var encryptedJson = File.ReadAllText(_profileFile);
        var json = SimpleDecrypt(encryptedJson);
        
        return JsonConvert.DeserializeObject<ProfileData>(json) ?? 
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