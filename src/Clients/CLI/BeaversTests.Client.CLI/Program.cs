using BeaversTests.Client.CLI.Auth;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;

var app = new CommandApp();

app.Configure(c =>
{
    c.SetApplicationName("bvrs");

    c.AddCommand<LoginCommand>(LoginCommand.CommandName);

    c.SetHelpProvider(new HelpProvider(c.Settings));
});

await app.RunAsync(args);