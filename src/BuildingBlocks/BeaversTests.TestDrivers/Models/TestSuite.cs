using System.Collections.Generic;

namespace BeaversTests.TestDrivers
{
    // TODO: Подумать над вложенностью
    public class TestSuite
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Test> Tests { get; set; }
    }
}

