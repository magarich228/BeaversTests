using System.Reflection;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

var testAssemblyPath = args.Single();

Console.WriteLine($"\nLoading test assembly: {testAssemblyPath}\n");

Console.WriteLine($"Context assemblies: {AppDomain.CurrentDomain.GetAssemblies().Length} " + 
                  string.Join(", ", AppDomain.CurrentDomain.GetAssemblies()
                      .Select(x => x.GetName().Name)));

var asm = Assembly.LoadFrom(testAssemblyPath);

Console.WriteLine($"Context assemblies: {AppDomain.CurrentDomain.GetAssemblies().Length} " + 
                  string.Join(", ", AppDomain.CurrentDomain.GetAssemblies()
                      .Select(x => x.GetName().Name)));

var runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

// var tests = runner.Load(asm, new Dictionary<string, object>());
var tests = runner.Load(testAssemblyPath, new Dictionary<string, object>());

LogTest(tests);

Console.WriteLine("\nExploring tests...\n");

tests = runner.ExploreTests(TestFilter.Empty);

LogTest(tests);

var results = runner.Run(TestListener.NULL, TestFilter.Empty);

Console.WriteLine($"\nSuccess: {results.PassCount} Failed: {results.FailCount} Total: {results.TotalCount}");

LogResult(results);

void LogTest(ITest test, int level = 1)
{
    Console.WriteLine(new string(' ', level) + $"{test.Name} Cases: {test.TestCaseCount}");
    
    foreach (var child in test.Tests)
    {
        LogTest(child, level + 1);
    }
}

void LogResult(ITestResult testResult, int level = 1)
{
    Console.WriteLine(new string(' ', level) + $"{testResult.Name}: {testResult.ResultState.Label} {testResult.Message}");

    foreach (var child in testResult.Children)
    {
        LogResult(child, level + 1);
    }
}