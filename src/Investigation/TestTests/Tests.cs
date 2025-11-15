using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace TestTests;

[TestFixture]
public class Tests
{
    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    [Test]
    public void TestLog()
    {
        var logger = new Logger<Tests>(new LoggerFactory());
        logger.LogInformation("test");
        
        Assert.Pass();
    }
}