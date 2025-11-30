using System.Collections.Generic;

namespace BeaversTests.Drivers.Abstractions
{
    public class TestSuite
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ICollection<Test> Tests { get; set; } = new List<Test>();
        public ICollection<TestSuite> TestSuites { get; set; } = new List<TestSuite>();
    }
}