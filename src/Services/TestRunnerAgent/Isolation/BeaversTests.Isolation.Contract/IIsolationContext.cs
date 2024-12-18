namespace BeaversTests.Isolation.Contract;

public interface IIsolationContext : IAsyncDisposable
{
    Task StartRunnerAsync(CancellationToken cancellationToken = default);
}