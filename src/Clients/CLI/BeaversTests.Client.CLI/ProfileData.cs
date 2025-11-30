namespace BeaversTests.Client.CLI;

internal class ProfileData
{
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime IssuedAt { get; set; }
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt.AddMinutes(-5); // Запас 5 минут
    public bool ShouldRefresh => DateTime.UtcNow >= ExpiresAt.AddMinutes(-30); // Обновление за 30 минут до истечения
}