using BeaversTests.Isolation.Contract;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace BeaversTests.Isolation.Docker;

[Strategy(Name)]
public class DockerIsolationStrategy() : IIsolationStrategy
{
    // TODO: from configuration
    private readonly string _imageName = "alpine";
    private const string Name = "Docker";

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

    public async Task<IIsolationContext> PrepareIsolationContextAsync(CancellationToken cancellationToken = default)
    {
        using var client = _dockerClientConfiguration.CreateClient();

        var images = await client.Images.ListImagesAsync(new(), cancellationToken);

        if (!images.Any(i => i.RepoTags.Any(t => t.Contains(_imageName))))
        {
            await client.Images.CreateImageAsync(new()
            {
                FromImage = _imageName,
                Tag = "latest"
            }, new AuthConfig(), new Progress<JSONMessage>(), cancellationToken);
        }

        var response = await client.Containers.CreateContainerAsync(
            new()
            {
                Image = "alpine",
                Tty = true,
                Cmd = new[] {"sh"},
            },
            cancellationToken);

        _containerId = response.ID;
        
        var started = await client.Containers.StartContainerAsync(
            _containerId,
            new(),
            cancellationToken);
        
        if (!started)
        {
            throw new Exception("Failed to start container");
        }
        
        // TODO: Копирование раннера в контейнер
        
        return new DockerIsolationContext(
            _containerId, 
            _dockerClientConfiguration);
    }
}