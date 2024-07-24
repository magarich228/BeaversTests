using System.Reflection;
using BeaversTests.TestDrivers.Models;

namespace BeaversTests.TestDrivers;

public interface ITestsExplorer
{
    IEnumerable<TestSuite> GetTestSuites(Assembly testsAssembly);
    IEnumerable<TestSuite> GetTestSuites(string testAssemblyPath);
}