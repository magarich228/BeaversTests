namespace BeaversTests.Isolation.Contract;

public interface IIsolationStrategy : IAsyncDisposable
{
    Task<bool> IsPossibleAsync(CancellationToken cancellationToken = default);
    Task PrepareIsolationContextAsync(CancellationToken cancellationToken = default);
}