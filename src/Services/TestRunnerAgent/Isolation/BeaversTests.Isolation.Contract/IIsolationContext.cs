namespace BeaversTests.Isolation.Contract;

public interface IIsolationContext : IAsyncDisposable
{
    Task<bool> IsAliveAsync();
}