using System;

namespace BeaversTests.Drivers.Abstractions
{
    public class TestResult
    {
        public string Message { get; set; }
        public Status ResultStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Output { get; set; }
        
        public enum Status : int
        {
            Unknown = 0,
            Skipped = 1,
            Success = 2,
            Warning = 3,
            Failure = 4,
            Cancelled = 5,
            Error = 6
        }
    }
}