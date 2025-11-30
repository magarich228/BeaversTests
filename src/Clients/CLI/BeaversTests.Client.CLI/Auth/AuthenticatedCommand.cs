using Spectre.Console;
using Spectre.Console.Cli;

namespace BeaversTests.Client.CLI.Auth;

/// <summary>
/// Base command for authenticated commands
/// </summary>
internal abstract class AuthenticatedCommand<TCommandSettings>(AuthService authService) : 
    Command<TCommandSettings> where TCommandSettings : CommandSettings
{
    private readonly AuthService _authService = authService ?? 
                                                throw new BeaversTestsCliException($"Failed to resolve {nameof(AuthService)}.");
    
    /// <summary>
    /// Execute base authenticated command. Use at the beginning of the execution of a child command.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="settings">The settings.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to abort the command.</param>
    /// <returns>An integer indicating whether the command executed successfully.</returns>
    protected override int Execute(CommandContext context, TCommandSettings settings, CancellationToken cancellationToken)
    {
        var authResult = _authService.RefreshOrLogin(cancellationToken);

        if (!authResult.Success)
        {
            throw new BeaversTestsCliException(authResult.Error ?? "Failed to authenticate.");
        }
        
        return 0;
    }
}

/// <summary>
/// Base command for authenticated commands
/// </summary>
internal abstract class AuthenticatedCommand(AuthService authService) : Command
{
    private readonly AuthService _authService = authService ?? 
                                                throw new BeaversTestsCliException($"Failed to resolve {nameof(AuthService)}.");
    
    /// <summary>
    /// Execute base authenticated command. Use at the beginning of the execution of a child command.
    /// </summary>
    /// <param name="context">The command context.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to abort the command.</param>
    /// <returns>An integer indicating whether the command executed successfully.</returns>
    protected override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        var authResult = _authService.RefreshOrLogin(cancellationToken);

        if (!authResult.Success)
        {
            throw new BeaversTestsCliException(authResult.Error ?? "Failed to authenticate.");
        }
        
        return 0;
    }
}