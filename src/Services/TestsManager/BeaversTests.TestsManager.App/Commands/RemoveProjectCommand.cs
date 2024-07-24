using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Exceptions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class RemoveProjectCommand
{
    public class Command : ICommand<Result>
    {
        public required Guid Id { get; set; }
    }
    
    public class Result { }
    
    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db)
        {
            RuleFor(c => c.Id)
                .NotEmpty()
                .MustAsync(async (id, token) => 
                    await db.TestProjects.AnyAsync(
                        t => t.Id == id, 
                        token))
                .WithMessage("Test project not found.");
        }
    }

    public class Handler(
        ITestsManagerContext db,
        ITestsStorageService testsStorageService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            // TODO: remove concrete project test package only
            var removedTestProject = await db.TestProjects
                .Include(t => t.TestPackages)
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            
            if (removedTestProject == null)
            {
                throw new Exception("Test project not found.");
            }
            
            db.TestProjects.Remove(removedTestProject);

            var testPackagesIds = removedTestProject.TestPackages
                ?.Select(t => t.Id)
                .ToList() ??
                                  throw new TestsManagerException("Test project test packages not found.");

            foreach (var testPackagesId in testPackagesIds)
            {
                await testsStorageService.RemoveTestAssemblyAsync(testPackagesId, cancellationToken);
            }

            if (await db.SaveChangesAsync(cancellationToken) < 1)
            {
                throw new TestsManagerException("Test project not deleted.");
            }
            
            return new Result();
        }
    }
}