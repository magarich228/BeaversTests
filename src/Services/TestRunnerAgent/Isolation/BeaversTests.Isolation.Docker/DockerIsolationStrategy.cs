using System.Text;
using BeaversTests.Isolation.Contracts;
using Docker.DotNet;
using Docker.DotNet.Models;
using ICSharpCode.SharpZipLib.Tar;

namespace BeaversTests.Isolation.Docker;

[Strategy(Name)]
public class DockerIsolationStrategy : IIsolationStrategy
{
    private readonly string _imageName = "beavers-tests-runner";
        
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
        
        var tarball = CreateTarballForDockerfileDirectory(Environment.CurrentDirectory);

        var imageBuildParams = new ImageBuildParameters()
        {
            Tags = new[] { _imageName },
            Dockerfile = "RuntimeDockerfile"
        };
        
        await client.Images.BuildImageFromDockerfileAsync(
            imageBuildParams,
            tarball,
        null,
        new Dictionary<string, string>(),
            new Progress<JSONMessage>(),
            cancellationToken);

        var containerParams = new CreateContainerParameters()
        {
            Image = _imageName,
            Name = "beavers-tests-runner",
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { "53999/tcp", new EmptyStruct() }
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { 
                        "53999/tcp", 
                        new List<PortBinding>
                        {
                            new() { HostPort = "53999" }
                        }
                    }
                }
            }
        };

        var response = await client.Containers.CreateContainerAsync(containerParams, cancellationToken);

        _containerId = response.ID;
        
        var started = await client.Containers.StartContainerAsync(
            _containerId,
            new ContainerStartParameters(),
            cancellationToken);
        
        if (!started)
        {
            throw new Exception("Failed to start container");
        }
        
        return new DockerIsolationContext(
            _containerId, 
            new DockerClientConfiguration(
                _dockerClientConfiguration.EndpointBaseUri,
                _dockerClientConfiguration.Credentials,
                _dockerClientConfiguration.DefaultTimeout,
                _dockerClientConfiguration.NamedPipeConnectTimeout,
                _dockerClientConfiguration.DefaultHttpRequestHeaders));
    }

    public ValueTask DisposeAsync()
    {
        _dockerClientConfiguration.Dispose();
        
        return ValueTask.CompletedTask;
    }
    
    private static Stream CreateTarballForDockerfileDirectory(string directory)
    {
        var tarball = new MemoryStream();
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);

        using var archive = new TarOutputStream(tarball, Encoding.UTF8);
        archive.IsStreamOwner = false;

        foreach (var file in files)
        {
            string tarName = file.Substring(directory.Length).Replace('\\', '/').TrimStart('/');
		
            var entry = TarEntry.CreateTarEntry(tarName);
            using var fileStream = File.OpenRead(file);
            
            entry.Size = fileStream.Length;
            archive.PutNextEntry(entry);

            byte[] localBuffer = new byte[32 * 1024];
            while (true)
            {   
                int numRead = fileStream.Read(localBuffer, 0, localBuffer.Length);
                if (numRead <= 0)
                    break;

                archive.Write(localBuffer, 0, numRead);
            }
		
            archive.CloseEntry();
        }
        
        archive.Close();

        tarball.Position = 0;
        return tarball;
    }
}