using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class RemoveTestPackageCommand
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
                    await db.TestPackages.AnyAsync(
                        t => t.Id == id, 
                        token))
                .WithMessage("Test package not found.");
        }
    }
    
    public class Handler(
        ITestsManagerContext db, 
        ITestsStorageService testsStorageService) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            // TODO: remove concrete project test package only
            var removedTestPackage = await db.TestPackages.FindAsync(
                new object?[] { request.Id }, 
                cancellationToken);
            
            if (removedTestPackage == null)
            {
                throw new Exception("Test package not found.");
            }
            
            db.TestPackages.Remove(removedTestPackage);
            
            await testsStorageService.RemoveTestAssemblyAsync(removedTestPackage.Id, cancellationToken);
            
            await db.SaveChangesAsync(cancellationToken);

            return new Result();
        }
    }
}