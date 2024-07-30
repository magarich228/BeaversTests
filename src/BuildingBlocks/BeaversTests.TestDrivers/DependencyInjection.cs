using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BeaversTests.TestDrivers;

// TODO: Split the assembly into a package for driver development and a assembly for services
public static class DependencyInjection
{
    public static IServiceCollection AddTestDrivers(this IServiceCollection services, IConfiguration configuration)
    {
        var testDriversConfiguration = new Configuration();
        configuration.Bind(testDriversConfiguration);
        
        var driversDirectory = new DirectoryInfo(testDriversConfiguration.TestDriversDirectory);

        var driverFiles = GetDriverFiles(driversDirectory);
        
        var keyedTestExplorerInterfaceType = typeof(ITestsExplorer<>);
        var testExplorerInterfaceType = typeof(ITestsExplorer);
        var driverKeyInterfaceType = typeof(IDriverKey);
        var keyProperty = typeof(IKeyedDriverService<>).GetProperty("DriverKey") ?? // TODO: move to constants or metadata
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
                // TODO: Log error
                Console.WriteLine(e);
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
                    // TODO: Log skip
                    continue;
                }

                var asmTestExplorerType = asmTestExplorerTypes
                    .FirstOrDefault(t => t.GetProperties()
                        .Any(p => p.Name == keyProperty.Name && 
                                  p.PropertyType.GetGenericTypeDefinition() == keyProperty.PropertyType.GetGenericTypeDefinition() &&
                                  p.PropertyType.GenericTypeArguments.First() == driverKeyType));

                if (asmTestExplorerType is null)
                {
                    // TODO: Log skip
                    continue;
                }
                
                services.AddKeyedScoped(testExplorerInterfaceType, driverKey.Key, asmTestExplorerType);
            }
        }

        services.AddSingleton<TestDriversResolver>();
        
        return services;
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