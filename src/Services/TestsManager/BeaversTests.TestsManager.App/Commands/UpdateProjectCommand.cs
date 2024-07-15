using AutoMapper;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Core.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Commands;

public class UpdateProjectCommand
{
    public class Command : ICommand<Result>
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
    }
    
    public class Result
    {
        public required TestProjectDto TestProject { get; init; }
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db)
        {
            RuleFor(c => c)
                .NotNull();
            
            RuleFor(c => c.Name)
                .NotEmpty()
                .NotNull()
                .MinimumLength(1)
                .MaximumLength(50);

            RuleFor(c => c.Description)
                .MaximumLength(1000);
            
            RuleFor(c => c.Id)
                .NotEmpty()
                .MustAsync((id, token) => db.TestProjects
                    .AnyAsync(x => x.Id == id, token))
                .WithMessage("Project with this id does not exist");
        }
    }
    
    public class Handler(
        ITestsManagerContext db,
        IMapper mapper) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
        {
            var testProject = mapper.Map<Command, TestProject>(command);

            var result = db.TestProjects.Update(testProject);

            if (await db.SaveChangesAsync(cancellationToken) == 0)
                throw new TestsManagerException("Failed to update project.");

            return new Result()
            {
                TestProject = mapper.Map<TestProject, TestProjectDto>(result.Entity)
            };
        }
    }
}