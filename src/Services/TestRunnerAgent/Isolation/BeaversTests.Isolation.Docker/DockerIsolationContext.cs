using BeaversTests.Isolation.Contract;
using Docker.DotNet;

namespace BeaversTests.Isolation.Docker;

public class DockerIsolationContext(
    string? containerId, 
    DockerClientConfiguration dockerClientConfiguration) : IIsolationContext
{
    public Task StartRunnerAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public async ValueTask DisposeAsync()
    {
        using var client = dockerClientConfiguration.CreateClient();

        if (containerId is not null)
        {
            await client.Containers.RemoveContainerAsync(containerId, new()
            {
                Force = true
            });
        }

        dockerClientConfiguration.Dispose();
    }
}