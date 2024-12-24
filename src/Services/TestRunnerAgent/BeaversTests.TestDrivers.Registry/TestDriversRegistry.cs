using System.Reflection;
using System.Runtime.Loader;
using BeaversTests.Common.Binary;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestDrivers.Registry;

public class TestDriversRegistry : IDisposable
{
    private readonly DirectoryInfo _driversDirectory;

    private const string DriverKeyPropertyName = "DriverKey";

    private readonly Type _keyedTestExplorerInterfaceType = typeof(ITestsExplorer<>);
    private readonly Type _testExplorerInterfaceType = typeof(ITestsExplorer);
    private readonly Type _driverKeyInterfaceType = typeof(IDriverKey);
    private readonly Type _keyedDriverServerInterfaceType = typeof(IKeyedDriverService<>);

    private readonly List<RegistrationContext> _contexts = new();
    private readonly IServiceCollection _services = new ServiceCollection();

    public TestDriversRegistry(RegistryConfiguration configuration)
    {
        _driversDirectory = new DirectoryInfo(configuration.DriversPath ??
                                              Path.GetTempPath() + "test-drivers-registry");

        if (!Path.Exists(configuration.DriversPath))
        {
            _driversDirectory.Create();
        }
    }

    public ITestsExplorer? GetDriverTestExplorer(string key, Guid agId, bool required = false)
    {
        var serviceKey = key + agId;

        using var provider = _services.BuildServiceProvider();

        return required
            ? provider.GetRequiredKeyedService<ITestsExplorer>(serviceKey)
            : provider.GetKeyedService<ITestsExplorer>(serviceKey);
    }

    public void Register(string key, Guid agId, TestDriverContent driverContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        
        var driverDirectory = GetDriverDirectory(key, agId);
        var context = new RegistrationContext()
        {
            Key = key,
            AgId = agId,
            DriverDirectory = driverDirectory,
            LoadContext = new AssemblyLoadContext(driverDirectory.Name, true)
        };

        _contexts.Add(context);
        
        RegisterFromDirectories(context, driverContent.Directories);
        RegisterFromFiles(context, driverContent.Files);
    }

    private void RegisterFromDirectories(RegistrationContext context, IEnumerable<BeaversTestsDirectory> directories)
    {
        foreach (var directory in directories)
        {
            RegisterFromDirectories(context, directory.Directories);
            RegisterFromFiles(context, directory.TestFiles);
        }
    }

    private void RegisterFromFiles(RegistrationContext context, IEnumerable<BeaversTestsFile> files)
    {
        foreach (var file in files)
        {
            if (TryLoadAssembly(context, file, out var assembly) &&
                assembly != null)
            {
                RegisterDriversFromAssembly(context, assembly);
            }
        }
    }

    private bool TryLoadAssembly(RegistrationContext context, BeaversTestsFile file, out Assembly? assembly)
    {
        if (!file.Name.EndsWith(".dll"))
        {
            assembly = null;

            return false;
        }

        try
        {
            using var ms = new MemoryStream(file.Content);
            assembly = context.LoadContext.LoadFromStream(ms);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            assembly = null;

            return false;
        }
    }

    private void RegisterDriversFromAssembly(RegistrationContext context, Assembly assembly)
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

            if (driverKey.Key != context.Key)
                continue;

            var serviceKey = driverKey.Key + context.AgId;

            _services.AddKeyedScoped(_testExplorerInterfaceType, serviceKey, asmTestExplorerType);

            return;
        }
    }

    private DirectoryInfo GetDriverDirectory(string key, Guid agId)
    {
        string directoryName = key;

        foreach (var invalidChar in Path.GetInvalidPathChars())
        {
            directoryName = directoryName.Replace($"{invalidChar}", string.Empty);
        }
        
        return _driversDirectory.CreateSubdirectory(agId + directoryName);
    }

    class RegistrationContext
    {
        public required string Key { get; init; }
        public required Guid AgId { get; init; }
        public required DirectoryInfo DriverDirectory { get; init; }
        public required AssemblyLoadContext LoadContext { get; init; }
    }

    public void Dispose()
    {
        _driversDirectory.Refresh();
        _driversDirectory.Delete(true);

        foreach (var context in _contexts)
        {
            if (context.DriverDirectory.Exists)
            {
                context.DriverDirectory.Delete(true);
            }
            
            context.LoadContext.Unload();
        }
    }
}