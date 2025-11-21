using BeaversTests.Drivers.Abstractions;
using BeaversTests.Drivers.Nunit;
using Newtonsoft.Json;

var testAssemblyPath = args.Single();

Console.WriteLine($"Loading test assembly: {testAssemblyPath}\n");

NUnitTestDriver driver = new NUnitTestDriver();

driver.Load(testAssemblyPath);

var testSuite = driver.Explore();

if (!testSuite.TestSuites.Any())
{
    Console.WriteLine("No tests found.");
    return;
}

Console.WriteLine("==================== LOADED TESTS =====================");

LogSuite(testSuite);

Console.WriteLine("====================== RUN TESTS ======================");

var listener = new TestListenerStub(Console.Out);

await driver.RunAsync(listener, null!);

Console.WriteLine("\nDone.");

void LogSuite(TestSuite suite, int level = 1)
{
    var tab = new string(' ', level);
    Console.WriteLine($"{tab}{suite.Id} {suite.Name} {suite.Description}");

    foreach (var test in suite.Tests)
    {
        Console.WriteLine($"{tab}{test.Id} {test.Name} {test.Description}");
    }

    foreach (var subSuite in suite.TestSuites)
    {
        LogSuite(subSuite, level + 1);
    }
}

class TestListenerStub(TextWriter writer) : ITestListener
{
    public Task SendAsync(TestEvent @event)
    {
        writer.WriteLine($"Event {@event.GetType().Name}: {JsonConvert.SerializeObject(@event, Formatting.Indented)}");

        return Task.CompletedTask;
    }
}