using System.Reflection;
using BeaversTests.Common.Binary;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestDrivers.Registry;

public class TestDriversRegistry
{
    private const string DriverKeyPropertyName = "DriverKey";

    private readonly Type _keyedTestExplorerInterfaceType = typeof(ITestsExplorer<>);
    private readonly Type _testExplorerInterfaceType = typeof(ITestsExplorer);
    private readonly Type _driverKeyInterfaceType = typeof(IDriverKey);
    private readonly Type _keyedDriverServerInterfaceType = typeof(IKeyedDriverService<>);

    private readonly IServiceCollection _services = new ServiceCollection();

    public ITestsExplorer? GetDriverTestExplorer(string key, Guid agId, bool required = false)
    {
        var serviceKey = key + agId;

        using var provider = _services.BuildServiceProvider();

        return required ? provider.GetRequiredKeyedService<ITestsExplorer>(serviceKey) : 
            provider.GetKeyedService<ITestsExplorer>(serviceKey);
    }

    public void Register(string key, Guid agId, TestDriverContent driverContent)
    {
        var assemblies1 = AppDomain.CurrentDomain.GetAssemblies();
        
        RegisterFromDirectories(driverContent.Directories, key, agId);
        RegisterFromFiles(driverContent.Files, key, agId);
        
        var assemblies2 = AppDomain.CurrentDomain.GetAssemblies();
    }

    private void RegisterFromDirectories(IEnumerable<BeaversTestsDirectory> directories, string key, Guid agId)
    {
        foreach (var directory in directories)
        {
            RegisterFromDirectories(directory.Directories, key, agId);
            RegisterFromFiles(directory.TestFiles, key, agId);
        }
    }

    private void RegisterFromFiles(IEnumerable<BeaversTestsFile> files, string key, Guid agId)
    {
        foreach (var file in files)
        {
            if (TryLoadAssembly(file, out var assembly) &&
                assembly != null)
            {
                RegisterDriversFromAssembly(assembly, key, agId);
            }
        }
    }

    private bool TryLoadAssembly(BeaversTestsFile file, out Assembly? assembly)
    {
        if (!file.Name.EndsWith(".dll"))
        {
            assembly = null;

            return false;
        }

        try
        {
            // assembly = Assembly.Load(file.Content);
            assembly = AppDomain.CurrentDomain.Load(file.Content);
            
            var references = assembly.GetReferencedAssemblies();
            
            foreach (var reference in references)
            {
                try
                {
                    AppDomain.CurrentDomain.Load(reference);
                }
                catch (Exception)
                {
                    // ignore
                }
            }
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            assembly = null;

            return false;
        }
    }

    private void RegisterDriversFromAssembly(Assembly assembly, string key, Guid agId)
    {
        if (assembly.FullName != null && !assembly.FullName.Contains("BeaversTests"))
        {
            return;
        }
        
        var keyProperty = _keyedDriverServerInterfaceType.GetProperty(DriverKeyPropertyName) ??
                          throw new ApplicationException("DriverKey property not found"); // TODO: custom exception

        var asmExportedTypes = assembly.GetExportedTypes();

        var asmDriverKeyTypes = asmExportedTypes
            .Where(t => t.IsAssignableTo(_driverKeyInterfaceType) &&
                        t is {IsAbstract: false, IsInterface: false})
            .ToArray();
        var asmTestExplorerTypes = asmExportedTypes
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == _keyedTestExplorerInterfaceType))
            .ToArray(); // TODO: select services by IKeyedDriverService<>

        foreach (var driverKeyType in asmDriverKeyTypes)
        {
            var driverKey = Activator.CreateInstance(driverKeyType) as IDriverKey;

            if (driverKey is null)
            {
                continue;
            }

            var asmTestExplorerType = asmTestExplorerTypes
                .FirstOrDefault(t => t.GetProperties()
                    .Any(p => p.Name == keyProperty.Name &&
                              p.PropertyType.GetGenericTypeDefinition() ==
                              keyProperty.PropertyType.GetGenericTypeDefinition() &&
                              p.PropertyType.GenericTypeArguments.First() == driverKeyType));

            if (asmTestExplorerType is null)
            {
                continue;
            }

            if (driverKey.Key != key)
                continue;

            var serviceKey = driverKey.Key + agId;

            _services.AddKeyedScoped(_testExplorerInterfaceType, serviceKey, asmTestExplorerType);

            return;
        }
    }
}