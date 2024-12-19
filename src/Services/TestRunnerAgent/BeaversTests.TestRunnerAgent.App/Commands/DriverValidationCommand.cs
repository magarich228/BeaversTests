using BeaversTests.Common.CQRS.Commands;
using BeaversTests.Isolation.Contract;
using BeaversTests.TestRunnerAgent.Core.Tasks;

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
    
    public class Handler : ICommandHandler<Command>
    {
        public Task Handle(Command command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}