using AutoMapper;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Exceptions;
using BeaversTests.TestsManager.Core.Models;
using BeaversTests.TestsManager.Core.Models.Enums;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BeaversTests.TestsManager.App.Commands;

public abstract class AddTestPackageCommand
{
    public class Command : ICommand<Result>
    {
        public required string Name { get; init; }
        public string? Description { get; init; }
        public required IEnumerable<byte[]> TestAssemblies { get; init; }
        public required IEnumerable<string> ItemPaths { get; init; }
        public string? TestPackageType { get; init; }
        public required Guid TestProjectId { get; init; }
    }

    public class Result
    {
        public Guid TestPackageId { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ITestsManagerContext db)
        {
            RuleFor(c => c.Name)
                .NotNull()
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(50)
                .MustAsync(async (c, name, token) => !await db.TestPackages
                    .AnyAsync(t => t.Name == name, token))
                .WithMessage("Test package with this name already exists.");

            RuleFor(c => c.Description)
                .MaximumLength(1000);

            RuleFor(c => c.TestPackageType)
                .MaximumLength(50)
                .IsEnumName(typeof(TestPackageType), false);

            RuleFor(c => c.ItemPaths)
                .NotEmpty()
                .NotNull();
            
            RuleFor(c => c.TestAssemblies)
                .NotEmpty()
                .NotNull()
                .Must((c, files) => files.Count().Equals(c.ItemPaths.Count()))
                .WithMessage("Test assemblies count doesn't match item paths count.");
            
            RuleFor(c => c.TestProjectId)
                .NotEmpty()
                .MustAsync(async (c, id, token) => await db.TestProjects
                    .AnyAsync(t => t.Id == id, token))
                .WithMessage("Test project not found.");
        }
    }

    public class Handler(
        ITestsStorageService testsStorageService,
        ITestsManagerContext db,
        IMapper mapper) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken = default)
        {
            var testPackage = mapper.Map<Command, BeaversTestPackage>(command);

            var addedTestPackage = await db.TestPackages.AddAsync(testPackage, cancellationToken);

            var testPackageId = addedTestPackage.Entity.Id;
            
            await testsStorageService.AddTestAssemblyAsync(
                testPackageId,
                command.TestAssemblies,
                command.ItemPaths,
                cancellationToken);

            if (await db.SaveChangesAsync(cancellationToken) < 1)
            {
                throw new TestsManagerException("Test package not added.");
            }

            return new Result
            {
                TestPackageId = testPackageId
            };
        }
    }
}