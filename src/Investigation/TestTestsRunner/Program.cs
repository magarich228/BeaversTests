using BeaversTests.Drivers.Abstractions;
using BeaversTests.Drivers.Nunit;

var testAssemblyPath = args.Single();

Console.WriteLine($"\nLoading test assembly: {testAssemblyPath}\n");

NUnitTestDriver driver = new NUnitTestDriver();

driver.Load(testAssemblyPath);

var testSuite = driver.Explore();

if (!testSuite.TestSuites.Any())
{
    Console.WriteLine("No tests found.");
    return;
}

LogSuite(testSuite);

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