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
            Console.WriteLine($"Drivers directory {_driversDirectory.FullName}.");
        }
    }

    public ITestsExplorer? GetDriverTestExplorer(string key, Guid agId, bool required = false)
    {
        var serviceKey = key + agId;

        Console.WriteLine($"Services count: {_services.Count}");
        using var provider = _services.BuildServiceProvider();

        return required
            ? provider.GetRequiredKeyedService<ITestsExplorer>(serviceKey)
            : provider.GetKeyedService<ITestsExplorer>(serviceKey);
    }

    public void Register(string key, Guid agId, TestDriverContent driverContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        ArgumentNullException.ThrowIfNull(driverContent);

        if (Guid.Empty == agId)
        {
            throw new ArgumentException($"Invalid {nameof(agId)}");
        }
    
        if (_contexts.Any(c => c.Key == key && c.AgId == agId))
        {
            throw new ArgumentException("Driver with this key and agId already registered.");
        }    

        var driverDirectory = GetDriverDirectory(key, agId);
        var context = new RegistrationContext()
        {
            Key = key,
            AgId = agId,
            DriverDirectory = driverDirectory,
            LoadContext = new RegistryAssemblyLoadContext(driverDirectory.Name, true)
        };

        _contexts.Add(context);

        SaveFiles(driverDirectory, driverContent.Files);
        SaveDirectories(driverDirectory, driverContent.Directories);

        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            Console.WriteLine($"Resolving: {args.Name}");
            
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.FullName == args.Name);

            if (asm is null && args.RequestingAssembly is not null)
            {
                asm = Assembly.LoadFile(args.RequestingAssembly.Location);
            }
            
            if (asm is not null)
                Console.WriteLine("Resolved");

            return asm;
        };
        // context.LoadContext.Resolving += (loadContext, name) =>
        // {
        //     Console.WriteLine($"Resolving: {name.FullName}");
        //     return loadContext.Assemblies.FirstOrDefault(a => a.FullName == name.FullName);
        // };

        RegisterFromDriverDirectory(context, driverDirectory);

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())//context.LoadContext.Assemblies)
        {
            RegisterDriversFromAssembly(context, asm);
        }
    }

    private void SaveDirectories(DirectoryInfo directoryInfo,
        IEnumerable<BeaversTestsDirectory> directories)
    {
        foreach (var directory in directories)
        {
            var directoryPath = Path.Combine(directoryInfo.FullName, directory.DirectoryName);

            var newDirectory = new DirectoryInfo(directoryPath);
            newDirectory.Create();

            SaveDirectories(newDirectory, directory.Directories);
            SaveFiles(newDirectory, directory.TestFiles);
        }
    }

    private void SaveFiles(DirectoryInfo directoryInfo,
        IEnumerable<BeaversTestsFile> files)
    {
        foreach (var file in files)
        {
            var filePath = Path.Combine(directoryInfo.FullName, file.Name);

            File.WriteAllBytes(filePath, file.Content);
        }
    }

    private void RegisterFromDriverDirectory(RegistrationContext context, DirectoryInfo directory)
    {
        foreach (var file in directory.EnumerateFiles())
        {
            if (TryLoadAssembly(context, file, out var assembly) &&
                assembly != null)
            {
                Console.WriteLine($"Загружена сборка: {assembly.FullName}");
            }
        }

        foreach (var subDirectory in directory.EnumerateDirectories())
        {
            RegisterFromDriverDirectory(context, subDirectory);
        }
    }

    private bool TryLoadAssembly(RegistrationContext context, FileInfo file, out Assembly? assembly)
    {
        // if (!file.Name.EndsWith(".dll"))
        // {
        //     assembly = null;
        //
        //     return false;
        // }

        try
        {
            assembly = Assembly.LoadFile(file.FullName);
            // assembly = context.LoadContext.LoadFromAssemblyPath(file.FullName);

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
                        t is { IsAbstract: false, IsInterface: false })
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
                Console.WriteLine("Driver key is null");
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
                Console.WriteLine("AsmTestExplorerType is null");
                continue;
            }

            if (driverKey.Key != context.Key)
            {
                Console.WriteLine($"Keys not equal. Driver key: {driverKey.Key}. Context key: {context.Key}");
                continue;
            }

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
        
        if (!_driversDirectory.TryDelete(true, out var exception))
        {
            Console.WriteLine(exception);
        }

        foreach (var context in _contexts)
        {
            if (context.DriverDirectory.Exists &&
                !context.DriverDirectory.TryDelete(true, out exception))
            {
                Console.WriteLine(exception);
            }

            context.LoadContext.Unload();
        }
        
        _contexts.Clear();
    }
}