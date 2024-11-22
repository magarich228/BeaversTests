using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Dtos.TestDriver;
using FluentValidation;

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
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            throw new NotImplementedException();
        }
    }

    public class Handler : ICommandHandler<Command, Result>
    {
        public Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}