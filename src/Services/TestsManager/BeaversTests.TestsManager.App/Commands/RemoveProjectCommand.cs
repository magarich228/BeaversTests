using AutoMapper;
using BeaversTests.Common.Application;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Core.TestProject;
using BeaversTests.TestsManager.Events.TestProject;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class RemoveProjectCommand
{
    public class Command : ICommand<Result>
    {
        public required Guid Id { get; set; }
        public string? UserId { get; internal set; }
    }
    
    public class Result { }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db,
            IUserService userService)
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .MustAsync(async (id, token) => 
                    await db.TestProjects
                        .Where(t => t.UserCreatorId == userService.GetCurrentUserId())
                        .AnyAsync(t => t.Id == id, token))
                .WithMessage("Test project not found.");
        }
    }

    public class Handler(
        IEventStore eventStore,
        IMapper mapper,
        IUserService userService,
        ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            logger.LogDebug("Test project {TestProjectId} deleted event has been received.", command.Id);

            command.UserId = userService.GetCurrentUserId();
            
            var projectAggregate = await eventStore.AggregateStreamAsync<TestProjectAggregate>(new AggregateInfo()
            {
                Id = command.Id
            }, cancellationToken);
            var deletedEvent = mapper.Map<TestProjectDeletedEvent>(command);
            
            projectAggregate.ApplyDeleted(deletedEvent);

            await eventStore.StoreAsync(projectAggregate, cancellationToken);

            return new Result();
        }
    }
}