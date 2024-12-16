using AutoMapper;
using BeaversTests.Common.Application;
using BeaversTests.Common.Binary;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos.TestDriver;
using BeaversTests.TestsManager.Core.TestDriver;
using BeaversTests.TestsManager.Events.TestDriver;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.Commands;

public class AddTestDriverCommand
{
    public class Command : ICommand<Result>
    {
        public required NewTestDriverDto TestDriver { get; init; }
    }

    public class Result
    {
        public required string TestDriverKey { get; init; }
        public required Guid AgId { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            // TODO: валидация
            RuleFor(c => c.TestDriver)
                .NotNull();

            RuleFor(c => c.TestDriver.Key)
                .NotNull()
                .NotEmpty();
        }
    }

    public class Handler(
        IEventStore eventStore,
        IDriversStorageWriteService driversStorageWriteService,
        IUserService userService,
        IMapper mapper,
        ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            logger.LogDebug("Adding test driver {Key}", command.TestDriver.Key);
            command.TestDriver.UserId = userService.GetCurrentUserId();

            var driverAggregate = new TestDriverAggregate();

            var addedEvent = mapper.Map<TestDriverAddedEvent>(command.TestDriver);
            
            driverAggregate.ApplyAdded(addedEvent);

            var contentDto = command.TestDriver.Content;
            var content = mapper.Map<NewTestDriverContentDto, TestDriverContent>(contentDto);
            
            await driversStorageWriteService.AddTestDriverAsync(
                command.TestDriver.Key, 
                command.TestDriver.AgId,
                content, 
                cancellationToken);
            
            await eventStore.StoreAsync(driverAggregate, cancellationToken);

            return new Result()
            {
                TestDriverKey = driverAggregate.Key,
                AgId = driverAggregate.Id
            };
        }
    }
}