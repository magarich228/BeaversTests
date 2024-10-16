using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestDrivers.Internal;

public class TestDriversRegistry
{
    private readonly Configuration _configuration;
    private readonly ILogger<TestDriversRegistry> _logger;
    private readonly IServiceCollection _serviceCollection;

    private const string DriverKeyPropertyName = "DriverKey";

    public IServiceProvider ServiceProvider => _serviceCollection.BuildServiceProvider();

    public TestDriversRegistry(
        IConfiguration configuration, 
        ILogger<TestDriversRegistry> logger)
    {
        _configuration = GetDriversConfiguration(configuration);
        _logger = logger;
        _serviceCollection = new ServiceCollection();
        
        // TODO: Вынести в отдельный инициализатор (регистрация в БД и в TestDriversRegistry)
        RegisterLocalDrivers();
    }
    
    // TODO: Add driver files strategy?
    public void RegisterDriver(IEnumerable<FileInfo> driverFiles)
    {
        // TODO: move to private fields.
        var keyedTestExplorerInterfaceType = typeof(ITestsExplorer<>);
        var testExplorerInterfaceType = typeof(ITestsExplorer);
        var driverKeyInterfaceType = typeof(IDriverKey);
        var keyProperty = typeof(IKeyedDriverService<>).GetProperty(DriverKeyPropertyName) ??
                          throw new ApplicationException("DriverKey property not found"); // TODO: custom exception

        foreach (var asmDriverFile in driverFiles)
        {
            Assembly asm;
            
            try
            {
                asm = Assembly.LoadFrom(asmDriverFile.FullName);
            }
            catch (Exception e)
            {
                _logger.LogDebug("Driver file {FileName} omitted due to:\n{ExceptionMessage}", asmDriverFile.FullName, e.Message);
                continue;
            }
            
            var asmExportedTypes = asm.GetExportedTypes();

            var asmDriverKeyTypes = asmExportedTypes
                .Where(t => t.IsAssignableTo(driverKeyInterfaceType) && 
                            t is { IsAbstract: false, IsInterface: false })
                .ToArray();
            var asmTestExplorerTypes = asmExportedTypes
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && 
                              i.GetGenericTypeDefinition() == keyedTestExplorerInterfaceType))
                .ToArray(); // TODO: select services by IKeyedDriverService<>

            foreach (var driverKeyType in asmDriverKeyTypes)
            {
                var driverKey = Activator.CreateInstance(driverKeyType) as IDriverKey;

                if (driverKey is null)
                {
                    _logger.LogDebug(
                        "Driver file {FileName} does not provide {DriverKeyTypeName}", 
                        asmDriverFile.FullName, 
                        driverKeyInterfaceType.Name);
                    
                    continue;
                }

                var asmTestExplorerType = asmTestExplorerTypes
                    .FirstOrDefault(t => t.GetProperties()
                        .Any(p => p.Name == keyProperty.Name && 
                                  p.PropertyType.GetGenericTypeDefinition() == keyProperty.PropertyType.GetGenericTypeDefinition() &&
                                  p.PropertyType.GenericTypeArguments.First() == driverKeyType));

                if (asmTestExplorerType is null)
                {
                    _logger.LogDebug(
                        "Driver file {FileName} does not provide {TestExplorerTypeName} for {DriverKey} driver key.",
                        asmDriverFile.FullName,
                        testExplorerInterfaceType.Name,
                        driverKey);
                    continue;
                }
                
                _serviceCollection.AddKeyedScoped(testExplorerInterfaceType, driverKey.Key, asmTestExplorerType);
            }
        }
    }

    private void RegisterLocalDrivers()
    {
        var driversDirectory = new DirectoryInfo(_configuration.TestDriversDirectory);
        
        if (string.IsNullOrWhiteSpace(_configuration.TestDriversDirectory) || !driversDirectory.Exists)
            throw new ApplicationException(
                "The mandatory test driver directory parameter is not provided from the configuration"); // TODO: add custom exception

        var driverFiles = GetDriverFiles(driversDirectory);
        
        RegisterDriver(driverFiles);
    }

    private Configuration GetDriversConfiguration(IConfiguration configuration)
    {
        var testDriversConfiguration = new Configuration();
        configuration.Bind(testDriversConfiguration);

        if (string.IsNullOrWhiteSpace(testDriversConfiguration.TestDriversDirectory))
            throw new ApplicationException(
                "The mandatory test driver directory parameter is not provided from the configuration"); // TODO: add custom exception
        
        return testDriversConfiguration;
    }
    
    private static IEnumerable<FileInfo> GetDriverFiles(DirectoryInfo testDriversDirectory)
    {
        var driverFiles = testDriversDirectory.EnumerateFiles()
            .Where(f => f.Extension.Equals(".dll")) // TODO: move to constants
            .ToList();

        foreach (var testDriversSubDirectory in testDriversDirectory.EnumerateDirectories())
        {
            driverFiles.AddRange(GetDriverFiles(testDriversSubDirectory));
        }

        return driverFiles;
    }
}