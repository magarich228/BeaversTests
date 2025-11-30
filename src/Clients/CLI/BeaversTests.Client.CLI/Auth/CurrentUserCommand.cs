using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.Client.CLI.Auth;

internal class CurrentUserCommand(AuthService authService) : AuthenticatedCommand(authService)
{
    private readonly AuthService _authService = authService;
    
    public const string CommandName = "user";
    
    protected override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        base.Execute(context, cancellationToken);

        var currentUserResult = _authService.GetCurrentUser(cancellationToken);

        if (!currentUserResult.Success)
        {
            AnsiConsole.WriteLine(currentUserResult.Error ?? "Failed to get current user.");

            return 1;
        }

        var currentUserJson = JsonConvert.SerializeObject(currentUserResult.UserInfo, Formatting.Indented);
        
        AnsiConsole.WriteLine(currentUserJson);
        
        return 0;
    }
}