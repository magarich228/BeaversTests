namespace BeaversTests.Isolation.Contracts;

public interface IIsolationContext : IAsyncDisposable
{
    Task<bool> IsAliveAsync(CancellationToken cancellationToken = default);
}