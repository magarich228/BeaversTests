using BeaversTests.Isolation.Contracts;
using BeaversTests.Isolation.Docker;

namespace BeaversTests.Isolation;

public class IsolationService
{
    // TODO: Убрать костыль
    private readonly DockerIsolationStrategy _dockerIsolationStrategy = new();
    
    public async Task<IIsolationStrategy> FindPossibleStrategy(List<string>? exclusionNames = null, CancellationToken cancellationToken = default)
    {
        // TODO: Выбор стратегии в зависимости от приоритета из конфигурации?
        var exclusionsExists = exclusionNames is not null && 
                               exclusionNames.Any();
        
        foreach (var isolationStrategy in GetIsolationStrategies())
        {
            if (exclusionsExists && isolationStrategy.IsExclusionStrategy(exclusionNames!))
            {
                await isolationStrategy.DisposeAsync();
                
                continue;
            }
            
            if (await isolationStrategy.IsPossibleAsync(cancellationToken))
            {
                return isolationStrategy;
            }
        }
        
        throw new IsolationException("All isolation strategies are not possible");
    }

    private IEnumerable<IIsolationStrategy> GetIsolationStrategies()
    {
        // TODO: Загружать только один раз
        foreach (var isolationStrategyType in AppDomain.CurrentDomain.GetAssemblies()
                     .Where(IsolationStrategyExtensions.IsIsolationModuleAssembly)
                     .SelectMany(asm => asm.GetExportedTypes())
                     .Where(IsolationStrategyExtensions.IsIsolationStrategyType))
        {
            IIsolationStrategy isolationStrategy = null!;
            
            try
            {
                isolationStrategy = (IIsolationStrategy)Activator.CreateInstance(isolationStrategyType)!;
            }
            catch (Exception ex)
            {
                // TODO: Logger
                Console.WriteLine($"Isolation strategy {isolationStrategyType.Name} " +
                                  $"instance creation failed: {ex.Message}");
            }
            
            yield return isolationStrategy;
        }
    }
}