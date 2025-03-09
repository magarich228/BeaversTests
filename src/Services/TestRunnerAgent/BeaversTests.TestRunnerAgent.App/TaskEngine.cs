using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Isolation;
using BeaversTests.Isolation.Contracts;
using BeaversTests.TestRunnerAgent.App.Commands;
using BeaversTests.TestRunnerAgent.Core.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerAgent.App;

public class TaskEngine(
    TasksContainer tasksContainer,
    IsolationService isolationService,
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<TaskEngine> logger) : IHostedService, IAsyncDisposable
{
    private const string TaskEngineDelayMsConfigurationKey = "TaskEngineDelayMs";
    
    private int _delayMilliseconds = 1000;
    private readonly CancellationTokenSource _workTaskCancellationTokenSource = new();
    private IIsolationContext? _isolationContext;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (configuration[TaskEngineDelayMsConfigurationKey] is not null &&
            int.TryParse(configuration[TaskEngineDelayMsConfigurationKey], out var delay))
        {
            _delayMilliseconds = delay;
        }
        
        await PrepareIsolationContext(cancellationToken);
        
        var workTask = new Task(ProcessTasks, _workTaskCancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        
        logger.LogInformation("Task engine is started");

        workTask.Start();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Task engine is stopped");

        await _workTaskCancellationTokenSource.CancelAsync();
    }

    private void ProcessTasks()
    {
        while (true)
        {
            if (!tasksContainer.TryGetNextTask(out var task))
            {
                Task.Delay(_delayMilliseconds)
                    .Wait();
                
                continue;
            }
            
            logger.LogInformation("Task engine is processing task {0}", task!.Type);

            if (_isolationContext is null)
            {
                PrepareIsolationContext()
                    .Wait();
            }
            
            ICommand command = task.Type switch
            {
                TaskType.DriverValidation => DriverValidationCommand.Command.Create(task, _isolationContext!),
                TaskType.TestPackageValidation => throw new NotImplementedException(), // TODO: TestPackageValidationCommand.Command.FromTask(task),
                TaskType.TestRun => throw new NotImplementedException(), // TODO: TestRunCommand.Command.FromTask(task),
                _ => throw new NotSupportedException($"Unsupported task type: {task.Type}")
            };

            using var scope = serviceProvider.CreateAsyncScope();
            var commandBus = scope.ServiceProvider.GetRequiredService<ICommandBus>();
            
            commandBus.SendAsync(command)
                .Wait();
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private async Task PrepareIsolationContext(CancellationToken cancellationToken = default)
    {
        var isolationStrategy = await isolationService.FindPossibleStrategy(cancellationToken: cancellationToken);
        var exclusions = new List<string>();

        while (_isolationContext is null)
        {
            try
            {
                _isolationContext = await isolationStrategy.PrepareIsolationContextAsync(cancellationToken);

                if (!await _isolationContext.IsAliveAsync(cancellationToken))
                {
                    throw new TestRunnerAgentException("Prepared isolation context is not alive.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to prepare isolation context: " + ex.Message);

                // TODO: custom exception
                var strategyName = isolationStrategy.GetName() ??
                                   throw new Exception("Failed to get strategy name");
                
                exclusions.Add(strategyName);
                
                isolationStrategy = await isolationService.FindPossibleStrategy(exclusions, cancellationToken);
            }
        }

        await isolationStrategy.DisposeAsync();
    }
    
    public async ValueTask DisposeAsync()
    {
        // TODO: реализовать гарантированное удаление контекста изоляции при любом способе завершения выполнения агента
        _workTaskCancellationTokenSource.Dispose();
        
        if (_isolationContext is not null)
            await _isolationContext.DisposeAsync();
    }
}