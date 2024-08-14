using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.CLI.TestsManagement;

public class AddTestPackageCommand : Command<AddTestPackageCommandSettings>
{
    public override int Execute(CommandContext context, AddTestPackageCommandSettings settings)
    {
        var result = settings.Validate();

        if (!result.Successful)
            return -1;

        Console.WriteLine(settings.TestPackagePath);
        
        return 0;
    }
}

public class AddTestPackageCommandSettings : CommandSettings
{
    public required string TestPackagePath { get; set; } 
    
    public override ValidationResult Validate()
    {
        if (string.IsNullOrWhiteSpace(TestPackagePath))
        {
            return ValidationResult.Error("TestPackagePath is required");
        }
        
        return base.Validate();
    }
}