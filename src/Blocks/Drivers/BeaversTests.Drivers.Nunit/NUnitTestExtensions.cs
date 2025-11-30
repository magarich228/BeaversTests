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
        // TODO: inner ex
        ArgumentNullException.ThrowIfNull(test, nameof(test));
        
        return test.TestType == TestAssemblyType;
    }
    
    public static bool IsTestFixture(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test, nameof(test));
        
        return test.TestType == TestFixtureType;
    }
    
    public static bool IsTestSuite(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test, nameof(test));
        
        return test.TestType == TestSuiteType;
    }

    public static bool IsTestCase(this ITest test)
    {
        ArgumentNullException.ThrowIfNull(test, nameof(test));
        
        return test.TestType == TestMethodType;
    }
    
    public static TestSuite ToTestSuite(this ITest nunitTest)
    {
        ArgumentNullException.ThrowIfNull(nunitTest, nameof(nunitTest));

        if (!nunitTest.IsTestSuite() && 
            !nunitTest.IsTestFixture() && 
            !nunitTest.IsTestAssembly())
        {
            throw new NunitDriverException(
                "One or more inner exceptions were thrown.", 
                new ArgumentException(
                    $"Test {nunitTest.FullName} is not a test suite, test fixture or assembly. Type: {nunitTest.TestType}."));
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
        ArgumentNullException.ThrowIfNull(nunitTest, nameof(nunitTest));

        if (!nunitTest.IsTestCase())
        {
            throw new NunitDriverException(
                "One or more inner exceptions were thrown.", 
                new ArgumentException(
                    $"Test {nunitTest.FullName} is not a test method type. Type: {nunitTest.TestType}."));
        }
        
        return new Test()
        {
            Id = nunitTest.Id,
            Name = nunitTest.Name,
            Description = $"{nunitTest.FullName}: {nunitTest.TestType}",
        };
    }
    
    public static TestResult ToTestResult(this ITestResult nunitResult)
    {
        ArgumentNullException.ThrowIfNull(nunitResult, nameof(nunitResult));
        
        return new TestResult()
        {
            ResultStatus = nunitResult.ResultState.MapState(),
            StartTime = nunitResult.StartTime,
            EndTime = nunitResult.EndTime,
            Message = nunitResult.Message,
            Output = nunitResult.Output
        };
    }

    public static Abstractions.TestOutput ToTestOutput(this NUnit.Framework.Interfaces.TestOutput nunitOutput)
    {
        ArgumentNullException.ThrowIfNull(nunitOutput, nameof(nunitOutput));
        
        return new Abstractions.TestOutput()
        {
            Output = $"{nunitOutput.Stream}: {nunitOutput.Text}"
        };
    }
    
    public static Abstractions.TestOutput ToTestOutput(this TestMessage nunitMessage)
    {
        ArgumentNullException.ThrowIfNull(nunitMessage, nameof(nunitMessage));
        
        return new Abstractions.TestOutput()
        {
            Output = $"{nunitMessage.Destination}: {nunitMessage.Message}"
        };
    }
    
    private static TestResult.Status MapState(this ResultState state) => state.Status switch
    {
        TestStatus.Passed => TestResult.Status.Success,
        TestStatus.Failed => MapFailedState(state),
        TestStatus.Skipped => TestResult.Status.Skipped,
        TestStatus.Inconclusive => TestResult.Status.Unknown,
        TestStatus.Warning => TestResult.Status.Warning,
        _ => TestResult.Status.Unknown
    };

    private static TestResult.Status MapFailedState(ResultState state)
    {
        // TODO: Test
        return state.Label switch
        {
            "Error" => TestResult.Status.Error,        // Неожиданные исключения
            "Cancelled" => TestResult.Status.Cancelled,// Отмененные тесты
            "Invalid" => TestResult.Status.Error,      // Некорректные тесты (NotRunnable)
            _ => TestResult.Status.Failure             // Обычные assertion failures
        };
    }
}