using System.Diagnostics;
using BeaversTests.Isolation.Contracts;

namespace BeaversTests.Isolation.HostNative;

public class HostIsolationContext(Process runnerProcess) : IIsolationContext
{
    public Task<bool> IsAliveAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(!runnerProcess.HasExited);
    }
    
    public ValueTask DisposeAsync()
    {
        runnerProcess.Kill();
        runnerProcess.Dispose();

        return ValueTask.CompletedTask;
    }
}