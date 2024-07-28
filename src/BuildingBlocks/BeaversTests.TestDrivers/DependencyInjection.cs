using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BeaversTests.TestDrivers;

public static class DependencyInjection
{
    public static IServiceCollection AddTestDrivers(this IServiceCollection services)
    {
        var testExplorerInterfaceType = typeof(ITestsExplorer<>);
        var driverKeyInterfaceType = typeof(IDriverKey);
        
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        foreach (var asm in assemblies)
        {
            var asmExporterTypes = asm.GetExportedTypes();

            var asmDriverKeyTypes = asmExporterTypes
                .Where(t => t.IsAssignableTo(driverKeyInterfaceType));
            var asmTestExplorers = asmExporterTypes
                .Where(t => t.IsAssignableTo(testExplorerInterfaceType));

            foreach (var driverKeyType in asmDriverKeyTypes)
            {
                var driverKey = Activator.CreateInstance(driverKeyType) as IDriverKey;
                // TODo: add keyed scoped services
            }
            
            // foreach (var testExplorerType in asmTestExplorers)
            // {
            //     services.TryAddKeyedScoped(testExplorerInterfaceType, testExplorerType);
            // }
        }
        
        return services;
    }
}