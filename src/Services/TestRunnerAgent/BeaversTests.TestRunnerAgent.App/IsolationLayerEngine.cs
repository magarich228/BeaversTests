using Microsoft.Extensions.Hosting;

namespace BeaversTests.TestRunnerAgent.App;

public class IsolationLayerEngine : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}