using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Execution;

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

    [Test]
    public void MessageAndOutputTest()
    {
        TestContext.WriteLine("=== Начало теста ===");
        
        TestExecutionContext.CurrentContext.SendMessage("Test destination.", "message from test");
        
        TestExecutionContext.CurrentContext.CurrentResult.OutWriter.WriteLine("Test output.");
        
        TestContext.Out.WriteLine("Test context out message");
        
        TestContext.Out.WriteLine("Это основное сообщение вывода");
        
        TestContext.Progress.WriteLine("Прогресс выполнения...");
        
        TestContext.WriteLine("=== Конец теста ===");
        
        Assert.Pass();
    }
}