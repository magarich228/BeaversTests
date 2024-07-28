using System.Reflection;
using BeaversTests.TestDrivers.Models;

namespace BeaversTests.TestDrivers;

public interface ITestsExplorer<TKey> : IKeyedDriverService<TKey> where 
    TKey : class, IDriverKey, new()
{
    IEnumerable<TestSuite> GetTestSuites(Assembly testsAssembly);
    IEnumerable<TestSuite> GetTestSuites(string testAssemblyPath);
}