using AutoMapper;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.Core.Models;
using FluentValidation;

namespace BeaversTests.TestsManager.App.Commands;

public class CreateProjectCommand
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

            return new Result {ProjectId = result.Entity.Id};
        }
    }
}