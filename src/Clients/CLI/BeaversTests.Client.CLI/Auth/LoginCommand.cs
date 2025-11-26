using System.ComponentModel;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.Client.CLI.Auth;

public class LoginCommand : Command<LoginCommand.LoginCommandSetting>
{
    private const string CredentialsFileName = "credentials";
    
    public const string CommandName = "login";

    protected override int Execute(CommandContext context, LoginCommandSetting settings, CancellationToken cancellationToken)
    {
        var credentialsFile = File.Open(CredentialsFileName, FileMode.Create, FileAccess.Write, FileShare.None);
        BinaryWriter writer = new(credentialsFile);
        
        
        writer.Write(JsonConvert.SerializeObject(settings));
        writer.Flush();
        
        credentialsFile.Dispose();
        writer.Dispose();
        
        return 0;
    }
    
    public class LoginCommandSetting : CommandSettings
    {
        [CommandOption("--email <EMAIL>")]
        [Description("Email of the user")]
        public required string Email { get; set; }
        
        [CommandOption("--password <PASSWORD>")]
        [Description("Password of the user")]
        public required string Password { get; set; }

        public override ValidationResult Validate()
        {
            return base.Validate();
        }
    }
}