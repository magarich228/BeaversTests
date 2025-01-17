using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Isolation.Contracts;
using BeaversTests.TestRunnerAgent.App.Abstractions;
using BeaversTests.TestRunnerAgent.Core.Tasks;
using BeaversTests.TestRunnerAgent.Events;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerAgent.App.Commands;

public abstract class DriverValidationCommand
{
    public class Command : ICommand
    {
        public required IIsolationContext IsolationContext { get; init; }
        public required string DriverKey { get; init; }
        public required Guid AgId { get; init; }

        public static Command Create(ITask task, IIsolationContext isolationContext)
        {
            var validationTask = task as DriverValidationTask ?? 
                                 throw new ArgumentException("Task is not DriverValidationTask");
            
            return new Command
            {
                IsolationContext = isolationContext,
                DriverKey = validationTask.DriverKey,
                AgId = validationTask.AgId
            };
        }
    }
    
    public class Handler(
        IDriversStorageReadService driversStorageReadService,
        IEventBus eventBus,
        ILogger<Handler> logger) : ICommandHandler<Command>
    {
        public async Task Handle(Command command, CancellationToken cancellationToken)
        {
            logger.LogDebug("Driver validation command received {0} ({1}).", command.DriverKey, command.AgId);
            
            var driverContent = await driversStorageReadService
                .GetTestDriverAsync(command.DriverKey, command.AgId, cancellationToken);
            
            var runnerDriverValidationCommand = new TestRunner.DriverValidationCommand()
            {
                DriverKey = command.DriverKey,
                AgId = command.AgId,
                Driver = driverContent
            };

            var result = await runnerDriverValidationCommand.SendAsync<TestRunner.DriverValidationCommand.Result>();

            var @event = new TestDriverValidationResultEvent()
            {
                Key = runnerDriverValidationCommand.DriverKey,
                AgId = runnerDriverValidationCommand.AgId,
                ValidationMessage = result.ValidationMessage ?? string.Empty,
                ValidationStatus = result.ValidationStatus
            };
            
            await eventBus.CommitAsync(cancellationToken, @event);
        }
    }
}