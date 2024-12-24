using BeaversTests.TestDrivers.Registry;
using Experiments;

var path = args.FirstOrDefault() ??
           @"C:\Users\kiril\RiderProjects\TMSNet\src\BuildingBlocks\Drivers\BeaversTests.NUnit.Driver\bin\Release\net8.0\publish";

var registry = new TestDriversRegistry(new RegistryConfiguration());

var entity = TestDriverContentFactory.CreateFromDirectory(path);

var key = new
{
    Key = "NUnit",
    AgId = Guid.NewGuid()
};

registry.Register(key.Key, key.AgId, entity);
var explorer = registry.GetDriverTestExplorer(key.Key, key.AgId);

Console.WriteLine(explorer);