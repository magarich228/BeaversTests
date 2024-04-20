using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
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

    public class Handler(ITestsManagerContext db) : ICommandHandler<Command, Result>
    {
        private readonly ITestsManagerContext _db = db;

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            // TODO:
            throw new NotImplementedException("Implement create new project command logic.");
        }
    }
}