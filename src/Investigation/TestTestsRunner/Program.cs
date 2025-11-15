using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

var testAssemblyPath = args.Single();

var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

var tests = runner.Load(testAssemblyPath, new Dictionary<string, object>());

LogTest(tests);

Console.WriteLine("\nExploring tests...\n");

tests = runner.ExploreTests(TestFilter.Empty);

LogTest(tests);

void LogTest(ITest test, int level = 1)
{
    Console.WriteLine(new string(' ', level) + $"{test.Name} Cases: {test.TestCaseCount}");
    
    foreach (var child in test.Tests)
    {
        LogTest(child, level + 1);
    }
}