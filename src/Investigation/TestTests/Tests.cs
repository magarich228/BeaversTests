using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
        logger.LogInformation("test test");
        
        Assert.Pass();
    }

    [Test]
    public void DITest()
    {
        var services = new ServiceCollection();
        
        Assert.That(services.Count, Is.EqualTo(0));
        
        services.Add(new ServiceDescriptor(typeof(object), new object()));
        
        Assert.That(services.Count, Is.EqualTo(1));
    }
}