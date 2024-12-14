using BeaversTests.Common.Application;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestRunnerController.Core;
using BeaversTests.TestRunnerController.Events;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestRunnerController.App.Commands;

public class CreateUserControllerKeyCommand
{
    public class Command : ICommand<Result> { }

    public class Result
    {
        public required string Key { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator() { }
    }

    public class Handler(
        IUserService userService,
        ControllerKeyDomainService controllerKeyDomainService,
        IEventStore eventStore,
        ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = userService.GetCurrentUserId();
            
            logger.LogDebug("Created user controller key for user {UserId}", userId);

            var key = controllerKeyDomainService.GenerateConnectionKey();
            
            var controllerUserKeyAggregate = new ControllerUserKeyAggregate();
            var createdEvent = new ControllerUserKeyCreatedEvent()
            {
                Id = Guid.NewGuid(),
                Key = key,
                OwnerId = userId
            };
            
            controllerUserKeyAggregate.ApplyCreated(createdEvent);

            await eventStore.StoreAsync(controllerUserKeyAggregate, cancellationToken);
            
            return new Result
            {
                Key = key
            };
        }
    }
}