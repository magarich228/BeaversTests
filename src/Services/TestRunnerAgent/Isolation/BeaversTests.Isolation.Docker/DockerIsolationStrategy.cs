using BeaversTests.Isolation.Contract;
using Docker.DotNet;

namespace BeaversTests.Isolation.Docker;

public class DockerIsolationStrategy() : IIsolationStrategy
{
    private readonly DockerClientConfiguration _dockerClientConfiguration = new();
    private string? _containerId;
    
    public async Task<bool> IsPossibleAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = _dockerClientConfiguration.CreateClient();
            var version = await client.System.GetSystemInfoAsync(cancellationToken);

            // TODO: Add logger
            Console.WriteLine($"{version.Name} {version.OSType} {version.Isolation} {version.ServerVersion}");

            await client.System.PingAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Docker strategy is not possible: " + ex.Message);

            return false;
        }
    }

    public Task PrepareIsolationContextAsync()
    {
        using var client = _dockerClientConfiguration.CreateClient();

        var response = client.Containers.CreateContainerAsync(new());

        throw new NotImplementedException();
    }

    public async ValueTask DisposeAsync()
    {
        using var client = _dockerClientConfiguration.CreateClient();

        if (_containerId is not null)
        {
            await client.Containers.RemoveContainerAsync(_containerId, new()
            {
                Force = true,
                RemoveVolumes = true,
                RemoveLinks = true
            });
        }
        
        _dockerClientConfiguration.Dispose();
    }
}