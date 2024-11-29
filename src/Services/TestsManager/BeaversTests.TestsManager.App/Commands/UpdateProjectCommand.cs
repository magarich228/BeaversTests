using AutoMapper;
using BeaversTests.Common.Application;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos.TestProject;
using BeaversTests.TestsManager.Core.TestProject;
using BeaversTests.TestsManager.Events.TestProject;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class UpdateProjectCommand
{
    public class Command : ICommand<Result>
    {
        public required Guid Id { get; init; }
        public string? UserId { get; internal set; }
        public required string Name { get; init; }
        public string? Description { get; init; }
    }
    
    public class Result
    {
        public required TestProjectDto TestProject { get; init; }
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db,
            IUserService userService) // TODO: Lay out the in shared rules
        {
            RuleFor(c => c)
                .NotNull();
            
            RuleFor(c => c.Name)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(50)
                .MustAsync(async (c, name, token) => !await db.TestProjects
                    .Where(t => t.UserCreatorId == userService.GetCurrentUserId())
                    .AnyAsync(t => t.Name == name, token))
                .WithMessage("Test project with this name already exists.");

            RuleFor(c => c.Description)
                .MaximumLength(1000);
            
            RuleFor(c => c.Id)
                .NotEmpty()
                .MustAsync((id, token) => db.TestProjects
                    .Where(t => t.UserCreatorId == userService.GetCurrentUserId())
                    .AnyAsync(x => x.Id == id, token))
                .WithMessage("Project with this id does not exist");
        }
    }
    
    public class Handler(
        IEventStore eventStore,
        IMapper mapper,
        IUserService userService,
        ILogger<Handler> logger) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("UpdateProjectCommand: handling {Command}", command.Id);

            command.UserId = userService.GetCurrentUserId();
            
            // TODO: remove aggregate stream
            var projectAggregate = await eventStore.AggregateStreamAsync<TestProjectAggregate>(new AggregateInfo()
            {
                Id = command.Id
            }, cancellationToken);
            
            var updatedEvent = mapper.Map<TestProjectUpdatedEvent>(command);
            
            projectAggregate.ApplyUpdated(updatedEvent);

            await eventStore.StoreAsync(projectAggregate, cancellationToken);

            return new Result()
            {
                TestProject = mapper.Map<TestProjectUpdatedEvent, TestProjectDto>(updatedEvent)
            };
        }
    }
}