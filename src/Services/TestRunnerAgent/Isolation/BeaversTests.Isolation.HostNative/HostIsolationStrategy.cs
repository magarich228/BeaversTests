using System.Diagnostics;
using System.Reflection;
using BeaversTests.Isolation.Contracts;

namespace BeaversTests.Isolation.HostNative;

[Strategy(Name)]
public class HostIsolationStrategy : IIsolationStrategy
{
    private const string Name = "Host";
    private readonly string _runnerPath = "BeaversTests.TestRunner.dll";

    public Task<bool> IsPossibleAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(_runnerPath));
    }

    public Task<IIsolationContext> PrepareIsolationContextAsync(CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "dotnet",
            Arguments = _runnerPath,
            UseShellExecute = false,
        };

        var runner = Process.Start(startInfo);
        
        if (runner is null ||
            runner.HasExited)
            throw new Exception("Failed to start test runner process.");

        return Task.FromResult<IIsolationContext>(new HostIsolationContext(runner));
    }
    
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}