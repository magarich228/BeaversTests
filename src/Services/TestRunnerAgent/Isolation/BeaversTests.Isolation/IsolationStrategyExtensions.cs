using System.Reflection;
using BeaversTests.Isolation.Contract;

namespace BeaversTests.Isolation;

public static class IsolationStrategyExtensions
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

    public static string? GetName(this IIsolationStrategy strategy)
    {
        return strategy
            .GetType()
            .GetCustomAttribute<StrategyAttribute>()?
            .Name;
    }
    
    internal static bool IsExclusionStrategy(this IIsolationStrategy strategy, IEnumerable<string> exclusionNames)
    {
        return exclusionNames.Any(e => strategy.GetName() == e);
    }
}