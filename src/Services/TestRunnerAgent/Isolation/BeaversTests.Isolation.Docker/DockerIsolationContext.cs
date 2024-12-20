using BeaversTests.Isolation.Contracts;
using Docker.DotNet;

namespace BeaversTests.Isolation.Docker;

public class DockerIsolationContext(
    string? containerId, 
    DockerClientConfiguration dockerClientConfiguration) : IIsolationContext
{
    public async Task<bool> IsAliveAsync(CancellationToken cancellationToken = default)
    {
        using var client = dockerClientConfiguration.CreateClient();

        var containerInfo = await client.Containers.InspectContainerAsync(
            containerId, 
            cancellationToken);

        return !containerInfo.State.Dead && !containerInfo.State.Paused;
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