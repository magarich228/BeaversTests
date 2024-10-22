using System.Collections.Generic;
using System.Reflection;

namespace BeaversTests.TestDrivers
{
    public interface ITestsExplorer<TKey> : IKeyedDriverService<TKey>, ITestsExplorer where 
        TKey : class, IDriverKey, new() { }

    public interface ITestsExplorer
    {
        // IEnumerable<TestSuite> GetTestSuites(Assembly testsAssembly);
        IEnumerable<TestSuite> GetTestSuites(string testFilePath);
    }
}