namespace BeaversTests.Drivers.Abstractions
{
    public abstract class TestEvent
    {
        public string TestId { get; set; }
    }
    
    public class TestStartedEvent : TestEvent { }

    public class TestFinishedEvent : TestEvent
    {
        public TestResult Result { get; set; }
    }
    
    public class TestOutputEvent : TestEvent
    {
        public TestOutput Output { get; set; }
    }
}