using BeaversTests.CLI.TestsManagement;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;

var app = new CommandApp();

app.Configure(c =>
{
    c.SetApplicationName("bvr");
    
    c.AddBranch("projects", configurator =>
    {
        configurator.AddCommand<GetAllProjectsCommand>("list");
    });
    
    c.AddBranch("tests", configurator =>
    {
        configurator.AddCommand<AddTestPackageCommand>("add");
    });

    c.SetHelpProvider(new HelpProvider(c.Settings));
});

await app.RunAsync(args);