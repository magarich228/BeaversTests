using System.Reflection;
using System.Runtime.Loader;

namespace BeaversTests.TestDrivers.Registry;

public class RegistryAssemblyLoadContext(string name, bool isCollectible) 
    : AssemblyLoadContext(name, isCollectible)
{
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        Console.WriteLine("Заргужается сборка: " + assemblyName.FullName);
        
        return base.Load(assemblyName);
    }
}