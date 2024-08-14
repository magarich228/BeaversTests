using BeaversTests.CLI.TestsManagement;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(c =>
{
    // c.AddBranch("projects", configurator =>
    // {
    //     
    // });
    
    c.AddBranch("tests", configurator =>
    {
        configurator.AddCommand<AddTestPackageCommand>("add");
    });
});

return await app.RunAsync(args);