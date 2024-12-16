using System.ComponentModel.Design;
using BeaversTests.Common.Binary;

namespace BeaversTests.TestDrivers.Registry;

public class TestDriversRegistry
{
    private readonly IServiceContainer _container =  new ServiceContainer();
    
    public void Register(string key, Guid agId, TestDriverContent driverContent)
    {
        
    }
}