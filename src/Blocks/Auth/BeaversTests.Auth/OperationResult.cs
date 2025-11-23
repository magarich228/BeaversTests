namespace BeaversTests.Auth;

public abstract class OperationResult
{
    public bool Success { get; set; }
    public string? Error { get; set; }
}