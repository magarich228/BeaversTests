using BeaversTests.Drivers.Abstractions;
using NUnit.Framework.Interfaces;
using ITestListener = NUnit.Framework.Interfaces.ITestListener;
using TestOutput = NUnit.Framework.Interfaces.TestOutput;

namespace BeaversTests.Drivers.Nunit;

public class NUnitTestListener : ITestListener
{
    public event EventHandler<TestEvent>? NUnitRunnerEvent;

    public void TestStarted(ITest test) =>
        NUnitRunnerEvent?.Invoke(this, new TestStartedEvent()
        {
            TestId = test.Id
        });

    public void TestFinished(ITestResult result) =>
        NUnitRunnerEvent?.Invoke(this, new TestFinishedEvent()
        {
            TestId = result.Test.Id,
            Result = result.ToTestResult()
        });

    public void TestOutput(TestOutput output) =>
        NUnitRunnerEvent?.Invoke(this, new TestOutputEvent()
        {
            TestId = output.TestId,
            Output = output.ToTestOutput()
        });

    public void SendMessage(TestMessage message) =>
        NUnitRunnerEvent?.Invoke(this, new TestOutputEvent()
        {
            TestId = message.TestId,
            Output = message.ToTestOutput()
        });
}