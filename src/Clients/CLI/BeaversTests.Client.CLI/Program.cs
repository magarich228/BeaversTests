using BeaversTests.Client.CLI;
using BeaversTests.Client.CLI.Auth;
using BeaversTests.Client.Http;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;

var services = new ServiceCollection();

services.AddBeaversTestsHttpClient();
services.AddSingleton<AuthService>();
services.AddSingleton<ProfileManager>();

var servicesRegistrar = new MicrosoftDIRegistrar(services);

var app = new CommandApp(servicesRegistrar);

app.Configure(c =>
{
    c.SetApplicationName("beavers");
    
    c.AddCommand<LoginCommand>(LoginCommand.CommandName);
    c.AddCommand<CurrentUserCommand>(CurrentUserCommand.CommandName);

    c.SetHelpProvider(new HelpProvider(c.Settings));
});

await app.RunAsync(args);