namespace BeaversTests.Isolation.Contract;

public interface IIsolationStrategy
{
    Task<bool> IsPossibleAsync(CancellationToken cancellationToken = default);
    Task<IIsolationContext> PrepareIsolationContextAsync(CancellationToken cancellationToken = default);
}