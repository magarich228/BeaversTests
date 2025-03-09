using Docker.DotNet.Models;

namespace BeaversTests.Isolation.Docker;

public class RunnerContainerConfig
{
    private readonly string _imageName = "beavers-tests-runner";
    private int _port = 53999;
    
    public RunnerContainerConfig()
    {
        Id = Guid.NewGuid();
        
        CreateContainerParameters = new CreateContainerParameters()
        {
            Image = _imageName,
            Name = $"beavers-tests-runner-{Id}",
            ExposedPorts = new Dictionary<string, EmptyStruct>
            {
                { $"{_port}/tcp", new EmptyStruct() }
            },
            HostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { 
                        $"{_port}/tcp", 
                        new List<PortBinding>
                        {
                            new() { HostPort = $"{_port}" }
                        }
                    }
                }
            }
        };
        
        ImageBuildParameters = new ImageBuildParameters()
        {
            Tags = new[] { _imageName },
            Dockerfile = "RuntimeDockerfile"
        };
    }

    public Guid Id { get; }
    public CreateContainerParameters CreateContainerParameters { get; }
    public ImageBuildParameters ImageBuildParameters { get; }
    
    // TODO: Смена портов.
}