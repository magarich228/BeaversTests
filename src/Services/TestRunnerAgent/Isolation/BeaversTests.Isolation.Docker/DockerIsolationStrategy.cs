using System.Diagnostics;
using BeaversTests.Isolation.Contracts;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace BeaversTests.Isolation.Docker;

[Strategy(Name)]
public class DockerIsolationStrategy() : IIsolationStrategy
{
    // TODO: from configuration
    private readonly string _imageName = "mcr.microsoft.com/dotnet/sdk:8.0";
    private readonly string _runnerPath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\BeaversTests.TestRunner\bin\Debug\net8.0\");
        
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
                Image = _imageName,
                Tty = true,
                Cmd = new[] {"bash"},
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
            },
            cancellationToken);

        _containerId = response.ID;

        var copyProcess = Process.Start(new ProcessStartInfo()
        {
            FileName = "docker",
            Arguments = $"cp {_runnerPath} {_containerId}:/runner",
        }) ?? throw new Exception("Failed to start copy runner files process.");

        copyProcess.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data != null)
            {
                Console.WriteLine(args.Data);
            }
        };
        
        await copyProcess.WaitForExitAsync(cancellationToken);
        
        if (copyProcess == null || copyProcess.ExitCode != 0)
            throw new Exception("Failed to copy runner files");
        
        var started = await client.Containers.StartContainerAsync(
            _containerId,
            new(),
            cancellationToken);

        var execResponse = await client.Exec.ExecCreateContainerAsync(
            _containerId, 
            new()
            {
                WorkingDir = "/runner",
                AttachStderr = true,
                AttachStdout = true,
                Cmd = new[] { "dotnet", "BeaversTests.TestRunner.dll" }
            }, cancellationToken);
        
        var startedExec = await client.Exec.StartWithConfigContainerExecAsync(
            execResponse.ID,
            new()
            {
                Detach = false,
                WorkingDir = "/runner",
                AttachStderr = true,
                AttachStdout = true,
                Cmd = new[] { "dotnet", "BeaversTests.TestRunner.dll" },
                Tty = true
            }, cancellationToken);

        // var output= await startedExec.ReadOutputToEndAsync(cancellationToken);
        // Console.WriteLine($"{output.stdout}\n{output.stderr}");
        
        if (!started || startedExec == null)
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
}