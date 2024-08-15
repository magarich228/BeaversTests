using System.ComponentModel;
using BeaversTests.Client;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.CLI.TestsManagement;

public class AddTestPackageCommand : Command<AddTestPackageCommand.AddTestPackageCommandSettings>
{
    public class AddTestPackageCommandSettings : CommandSettings
    {
        [CommandOption("-n|--name <NAME>")]
        [Description("Name of the test package.")]
        public required string Name { get; set; }

        [CommandOption("-d|--description <DESCRIPTION>")]
        [Description("Description of the test package.")]
        public string Description { get; set; } = null!;
        
        [CommandOption("--test-package-dir <TEST_PACKAGE_DIR>")]
        [Description("Path to the test package.")]
        public required string TestPackageDirectory { get; set; }
        
        // TODO: Get types from tests manager
        [CommandOption("--test-package-type <TEST_PACKAGE_TYPE>")]
        [Description("Type of the test package.")]
        public string TestPackageType { get; set; } = null!;
        
        [CommandOption("--test-project-id <TEST_PROJECT_ID>")]
        [Description("Id of the test project.")]
        public required Guid TestProjectId { get; set; }

        public override ValidationResult Validate()
        {
            if (!Directory.Exists(TestPackageDirectory))
            {
                return ValidationResult.Error("Test package directory not found.");
            }
            
            return base.Validate();
        }
    }
    
    public override int Execute(CommandContext context, AddTestPackageCommandSettings settings)
    {
        // TODO: move configuration to configuration branch
        var configuration = new Configuration()
        {
            ApiGatewayUrl = "http://localhost:5174/"
        };
        
        // TODO: DI?
        var client = new TestsManagerClient(configuration);
        
        var response = client.AddTestPackageFromDirectoryAsync(new NewTestPackageFromDirectoryDto()
        {
            Directory = new DirectoryInfo(settings.TestPackageDirectory),
            Name = settings.Name,
            Description = settings.Description,
            TestPackageType = settings.TestPackageType,
            TestProjectId = settings.TestProjectId
        }).Result;
        
        if (response is null)
        {
            AnsiConsole.MarkupLine("[red]Failed to add test package.[/]");
            return 1;
        }
        
        AnsiConsole.MarkupLine($"[green]Test package added. Id: {response.TestPackageId}[/]");
        
        return 0;
    }
}