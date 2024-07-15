using AutoMapper;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Core.Models;
using FluentValidation;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class CreateProjectCommand
{
    public class Command : ICommand<Result>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class Result
    {
        public Guid ProjectId { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(50);

            RuleFor(c => c.Description)
                .MaximumLength(1000);
        }
    }

    public class Handler(
        ITestsManagerContext db,
        IMapper mapper) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = mapper.Map<Command, TestProject>(request);

            var result = await db.TestProjects.AddAsync(project, cancellationToken);

            if (await db.SaveChangesAsync(cancellationToken) == 0)
                throw new TestsManagerException("Failed to create project.");

            return new Result {ProjectId = result.Entity.Id};
        }
    }
}