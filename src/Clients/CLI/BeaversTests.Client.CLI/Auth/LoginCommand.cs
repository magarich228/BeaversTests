using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.Client.CLI.Auth;

// ReSharper disable once ClassNeverInstantiated.Global
internal class LoginCommand(AuthService authService) : Command<LoginCommand.LoginCommandSetting>
{
    private readonly AuthService _authService = authService ?? 
                                               throw new BeaversTestsCliException($"Failed to resolve {nameof(AuthService)}");

    public const string CommandName = "login";

    protected override int Execute(
        CommandContext context, 
        LoginCommandSetting settings,
        CancellationToken cancellationToken)
    {
        var authResult = _authService.Login(settings.CredentialsFile, cancellationToken);

        if (!authResult.Success)
        {
            AnsiConsole.WriteLine(authResult.Error ?? "Failed to login");
            return 1;
        }
        
        AnsiConsole.WriteLine("{ \"success\": true }");
        
        return 0;
    }
    
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LoginCommandSetting : CommandSettings
    {
        [CommandOption("--credentials-file <CREDENTIALS_FILE>")]
        [Description(@"
Path to the credentials file. The alternative is an environment variable named BEAVERS_CREDENTIALS_FILE.
The credentials file is a JSON file with the following structure (UTF-8):
{
    ""email"": ""EMAIL"",
    ""password"": ""PASSWORD""
}

Alternatively, it can be set by environment variables BEAVERS_USER_EMAIL and BEAVERS_USER_PASSWORD.")]
        public required string? CredentialsFile { get; set; }

        public override ValidationResult Validate()
        {
            var credentialsFilePath = CredentialsFile ??
                                      Environment.GetEnvironmentVariable(AuthService.CredentialsFileEnvironmentVariable);

            var beaversUserEmailExist =
                Environment.GetEnvironmentVariable(AuthService.BeaversUserEmailEnvironmentVariable) is not null;
            var beaversUserPasswordExist =
                Environment.GetEnvironmentVariable(AuthService.BeaversUserPasswordEnvironmentVariable) is not null;

            if (credentialsFilePath is null && (!beaversUserEmailExist || !beaversUserPasswordExist))
            {
                return ValidationResult.Error(
                    "Credentials file path, user email, user password variables are not set.");
            }

            if (credentialsFilePath is not null && !File.Exists(credentialsFilePath))
            {
                return ValidationResult.Error($"Credentials file does not exist. ({credentialsFilePath})");
            }

            return ValidationResult.Success();
        }
    }
}