using NUnit.Framework.Interfaces;

namespace BeaversTests.NUnit.Driver;

public static class TestsExtensions
{
    public static bool IsTestFixture(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.TestType == "TestFixture";
    }
    
    public static bool IsTestSuite(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.TestType == "TestSuite";
    }

    public static bool IsTestCase(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.TestType == "TestMethod";
    }

    public static bool IsRoot(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        return test.Parent is null;
    }

    public static ITest GetTestsRoot(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        var root = test;
        
        while (!(root ?? 
                 throw new NUnitDriverException("Root test of argument is null."))
               .IsRoot())
        {
            root = root.Parent;
        }
        
        return root;
    }

    public static IList<ITest> ToFullList(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        var root = test.GetTestsRoot();

        return ToChildList(root);
    }

    public static IList<ITest> ToChildList(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test);
        
        List<ITest> resultTests = new List<ITest>();

        AddNestedTests(resultTests, test);

        return resultTests;

        void AddNestedTests(List<ITest> result, params ITest[] nestedTests)
        {
            result.AddRange(nestedTests);

            var childTests = nestedTests
                .SelectMany(t => t.Tests)
                .ToArray();

            if (childTests.Any())
            {
                AddNestedTests(result, childTests);
            }
        }
    }
}