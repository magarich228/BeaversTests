using BeaversTests.Client;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.CLI.TestsManagement;

public class GetAllProjectsCommand : Command<GetAllProjectsCommand.GetAllProjectCommandSettings>
{
    public class GetAllProjectCommandSettings : CommandSettings
    {
        public override ValidationResult Validate()
        {
            return base.Validate();
        }
    }

    public override int Execute(CommandContext context, GetAllProjectCommandSettings settings)
    {
        // TODO: move configuration to configuration branch
        var configuration = new Configuration()
        {
            ApiGatewayUrl = "http://localhost:5174/"
        };

        // TODO: DI?
        var client = new TestsManagerClient(configuration);

        var projects = client.GetTestProjectsAsync().Result;

        if (projects is null || !projects.Any())
        {
            Console.WriteLine("No projects found.");
        }
        
        foreach (var project in projects)
        {
            Console.WriteLine($"{project.Id} - {project.Name} - {project.Description}");
        }
        
        return 0;
    }
}