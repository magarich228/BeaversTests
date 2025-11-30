using System.ComponentModel;
using System.Text;
using BeaversTests.Auth.Public;
using BeaversTests.Client.Http;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.Client.CLI.Auth;

// ReSharper disable once ClassNeverInstantiated.Global
internal class LoginCommand(
    IBeaversTestsAuthClient authClient,
    ProfileManager profileManager) : Command<LoginCommand.LoginCommandSetting>
{
    private const string CredentialsFileName = "credentials";

    private readonly IBeaversTestsAuthClient _authClient = authClient ??
                                                           throw new BeaversTestsCliException(
                                                               $"Failed to resolve {nameof(IBeaversTestsAuthClient)}");
    private readonly ProfileManager _profileManager = profileManager ??
                                                      throw new BeaversTestsCliException(
                                                          $"Failed to resolve {nameof(ProfileManager)}");

    public const string CommandName = "login";

    protected override int Execute(
        CommandContext context, 
        LoginCommandSetting settings,
        CancellationToken cancellationToken)
    {
        var credentialsFilePath = settings.CredentialsFile ??
                                  Environment.GetEnvironmentVariable(LoginCommandSetting
                                      .CredentialsFileEnvironmentVariable);

        var userEmail = Environment.GetEnvironmentVariable(LoginCommandSetting.BeaversUserEmailEnvironmentVariable);
        var userPassword =
            Environment.GetEnvironmentVariable(LoginCommandSetting.BeaversUserPasswordEnvironmentVariable);

        if (!TryGetLoginModel(
                credentialsFilePath,
                userEmail,
                userPassword,
                out var loginModel,
                out var errorMessage,
                out var exitCode))
        {
            AnsiConsole.WriteLine(errorMessage ?? string.Empty);
            return exitCode!.Value;
        }

        var authResult = _authClient.LoginAsync(loginModel!, cancellationToken)
            .GetAwaiter()
            .GetResult();

        if (!authResult.Success)
        {
            AnsiConsole.WriteLine($"Failed to login ({authResult.StatusCode}): {authResult.Error}");
            return 1;
        }

        if (!authResult.Data?.Success ?? throw new BeaversTestsCliException($"Failed to get login result. {nameof(authResult.Data)} is null"))
        {
            AnsiConsole.WriteLine($"Failed to login: {authResult.Data.Error}");
            return 1;
        }
        
        AnsiConsole.WriteLine("Login successful.");
        
        _profileManager.SaveTokens(authResult.Data);
        
        return 0;
    }

    private bool TryGetLoginModel(
        string? credentialsFilePath,
        string? userEmail,
        string? userPassword,
        out LoginRequest? loginModel,
        out string? errorMessage,
        out int? exitCode)
    {
        if (credentialsFilePath is not null)
        {
            using var credentialsFile =
                File.Open(CredentialsFileName, FileMode.Create, FileAccess.Write, FileShare.None);

            using var streamReader = new StreamReader(credentialsFile, Encoding.UTF8);
            using var jsonReader = new JsonTextReader(streamReader);
            var serializer = JsonSerializer.CreateDefault();

            loginModel = serializer.Deserialize<LoginRequest>(jsonReader);

            if (loginModel is null)
            {
                errorMessage = "Credentials deserialization failed.";
                exitCode = 1;

                return false;
            }
        }
        else
        {
            if (userEmail is null || userPassword is null)
            {
                exitCode = 1;
                errorMessage = "Credentials env variables are not set.";
                loginModel = null;

                return false;
            }

            loginModel = new LoginRequest(userEmail, userPassword);
        }

        errorMessage = null;
        exitCode = null;

        return true;
    }

    public class LoginCommandSetting : CommandSettings
    {
        public const string CredentialsFileEnvironmentVariable = "BEAVERS_CREDENTIALS_FILE";
        public const string BeaversUserEmailEnvironmentVariable = "BEAVERS_USER_EMAIL";
        public const string BeaversUserPasswordEnvironmentVariable = "BEAVERS_USER_PASSWORD";

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
                                      Environment.GetEnvironmentVariable(CredentialsFileEnvironmentVariable);

            var beaversUserEmailExist =
                Environment.GetEnvironmentVariable(BeaversUserEmailEnvironmentVariable) is not null;
            var beaversUserPasswordExist =
                Environment.GetEnvironmentVariable(BeaversUserPasswordEnvironmentVariable) is not null;

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