namespace BeaversTests.Client.CLI;

internal class ProfileCredentialsData
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime IssuedAt { get; set; }
    public string? CredentialsFilePath { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool ShouldRefresh => DateTime.UtcNow >= ExpiresAt.AddMinutes(-30); // Обновление за 30 минут до истечения
}