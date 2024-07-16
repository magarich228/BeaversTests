using AutoMapper;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Core.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class UpdateProjectCommand
{
    public class Command : ICommand<Result>
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Description { get; init; }
    }
    
    public class Result
    {
        public required TestProjectDto TestProject { get; init; }
    }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db) // TODO: Lay out the in shared rules
        {
            RuleFor(c => c)
                .NotNull();
            
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