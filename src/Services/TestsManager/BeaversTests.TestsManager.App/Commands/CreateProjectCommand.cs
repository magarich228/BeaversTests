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

public abstract class CreateProjectCommand
{
    public class Command : ICommand<Result>
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string? UserId { get; internal set; }
        public required string Name { get; init; }
        public string? Description { get; init; }
    }

    public class Result
    {
        public Guid TestProjectId { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db, 
            IUserService userService)
        {
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
            logger.LogDebug("Create test project command handler called.");
            
            var projectAggregate = new TestProjectAggregate();

            command.UserId = userService.GetCurrentUserId();
            var createdEvent = mapper.Map<TestProjectAddedEvent>(command);
            
            projectAggregate.ApplyCreated(createdEvent);

            await eventStore.StoreAsync(projectAggregate, cancellationToken);

            return new Result()
            {
                TestProjectId = projectAggregate.Id
            };
        }
    }
}