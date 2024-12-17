using System.Reflection;
using BeaversTests.Isolation.Contract;

namespace BeaversTests.TestRunnerAgent.App;

internal static class IsolationStrategyExtensions
{
    private static readonly Type IsolationStrategyType = typeof(IIsolationStrategy);

    internal static bool IsIsolationStrategyType(this Type type)
    {
        return type.GetCustomAttribute<StrategyAttribute>() != null &&
               type.IsAssignableTo(IsolationStrategyType);
    }

    internal static bool IsIsolationModuleAssembly(this Assembly assembly)
    {
        return assembly.GetCustomAttribute<IsolationModuleAttribute>() != null;
    }
}