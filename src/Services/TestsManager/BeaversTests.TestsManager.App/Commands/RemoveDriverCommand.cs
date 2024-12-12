using AutoMapper;
using BeaversTests.Common.Application;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Core.TestDriver;
using BeaversTests.TestsManager.Events.TestDriver;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.Commands;

public class RemoveDriverCommand
{
    public class Command : ICommand<Result>
    {
        public required string Key { get; init; }
        public required Guid AgId { get; init; }
        public string? UserId { get; internal set; }
    }

    public class Result { }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator(
            ITestsManagerContext db, 
            IUserService userService)
        {
            RuleFor(c => c.Key)
                .NotNull()
                .NotEmpty()
                .MaximumLength(25);


            RuleFor(c => c.AgId)
                .NotNull()
                .NotEmpty();
            
            RuleFor(c => c)
                .MustAsync(async (command, token) => await db.TestDrivers
                    .Where(d => d.UserCreatorId == userService.GetCurrentUserId())
                    .AnyAsync(d => d.Key == command.Key && d.AgId == command.AgId, token))
                .WithMessage(c => $"Test driver {c.Key} ({c.AgId}) not found.");
        }
    }
    
    public class Handler(
        IEventStore eventStore,
        IMapper mapper,
        IUserService userService,
        IDriversStorageWriteService driversStorageWriteService,
        ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            logger.LogDebug("Test driver {TestDriverKey} deleted event has been received.", command.Key);
            
            command.UserId = userService.GetCurrentUserId();
            
            // TODO: remove aggregate stream
            var driverAggregate = await eventStore.AggregateStreamAsync<TestDriverAggregate>(new AggregateInfo()
            {
                Id = command.AgId
            }, cancellationToken);
            
            var deletedEvent = mapper.Map<TestDriverRemovedEvent>(command);
            
            driverAggregate.ApplyDeleted(deletedEvent);

            await driversStorageWriteService.RemoveTestDriverAsync(driverAggregate.Key, cancellationToken);
            await eventStore.StoreAsync(driverAggregate, cancellationToken);
            
            return new Result();
        }
    }
}