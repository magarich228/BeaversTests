namespace BeaversTests.Isolation.Contracts;

public interface IIsolationStrategy : IAsyncDisposable
{
    Task<bool> IsPossibleAsync(CancellationToken cancellationToken = default);
    Task<IIsolationContext> PrepareIsolationContextAsync(CancellationToken cancellationToken = default);
}