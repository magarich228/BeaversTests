using BeaversTests.Auth.AspNetCore.Shared;

namespace BeaversTests.Manager.Api;

public class VerificationResult
{
    public required bool IsAuthenticated { get; init; }
    public required UserInfo UserInfo { get; init; }
    public required DateTime Timestamp { get; init; }
}