using BeaversTests.Drivers.Abstractions;
using NUnit.Framework.Interfaces;

namespace BeaversTests.Drivers.Nunit;

public static class NUnitTestExtensions
{
    private const string TestAssemblyType = "Assembly";
    private const string TestFixtureType = "TestFixture";
    private const string TestSuiteType = "TestSuite";
    private const string TestMethodType = "TestMethod";
    
    public static bool IsTestAssembly(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.TestType == TestAssemblyType;
    }
    
    public static bool IsTestFixture(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.TestType == TestFixtureType;
    }
    
    public static bool IsTestSuite(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.TestType == TestSuiteType;
    }

    public static bool IsTestCase(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.TestType == TestMethodType;
    }
    
    public static TestSuite ToTestSuite(this ITest nunitTest)
    {
        ArgumentNullException.ThrowIfNull(nunitTest);

        if (!nunitTest.IsTestSuite() && 
            !nunitTest.IsTestFixture() && 
            !nunitTest.IsTestAssembly())
        {
            throw new ArgumentException($"Test {nunitTest.FullName} is not a test suite, test fixture or assembly. Type: {nunitTest.TestType}");
        }
        
        return new TestSuite()
        {
            Id = nunitTest.Id,
            Name = nunitTest.Name,
            Description = $"{nunitTest.FullName}: {nunitTest.TestType}",
        };
    }

    public static Test ToTestCase(this ITest nunitTest)
    {
        ArgumentNullException.ThrowIfNull(nunitTest);

        if (!nunitTest.IsTestCase())
        {
            throw new ArgumentException($"Test {nunitTest.FullName} is not a test method type. Type: {nunitTest.TestType}");
        }
        
        return new Test()
        {
            Id = nunitTest.Id,
            Name = nunitTest.Name,
            Description = $"{nunitTest.FullName}: {nunitTest.TestType}",
        };
    }
}