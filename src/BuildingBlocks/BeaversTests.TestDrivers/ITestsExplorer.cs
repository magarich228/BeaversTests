using System.Reflection;
using BeaversTests.TestDrivers.Models;

namespace BeaversTests.TestDrivers;

public interface ITestsExplorer<TKey> : IKeyedDriverService<TKey>, ITestsExplorer where 
    TKey : class, IDriverKey, new() { }

public interface ITestsExplorer
{
    IEnumerable<TestSuite> GetTestSuites(Assembly testsAssembly);
    IEnumerable<TestSuite> GetTestSuites(string testAssemblyPath);
}