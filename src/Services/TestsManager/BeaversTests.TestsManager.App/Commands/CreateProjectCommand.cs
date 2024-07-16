using AutoMapper;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Core.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class CreateProjectCommand
{
    public class Command : ICommand<Result>
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
    }

    public class Result
    {
        public Guid TestProjectId { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db)
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(50)
                .MustAsync(async (c, name, token) => !await db.TestProjects
                    .AnyAsync(t => t.Name == name, token))
                .WithMessage("Test project with this name already exists.");

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

            return new Result {TestProjectId = result.Entity.Id};
        }
    }
}