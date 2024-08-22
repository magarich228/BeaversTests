using AutoMapper;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
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
        public required NewTestPackageDto TestPackage { get; init; }
    }

    public class Result
    {
        public Guid TestPackageId { get; init; }
    }

    public class Validator : AbstractValidator<Command>
    {
        // TODO: move to configuration
        public const long MaxFileSize = 5 * 1024 * 1024; // bytes
        
        public Validator(ITestsManagerContext db)
        {
            // TODO: test validator;
            RuleFor(c => c.TestPackage)
                .NotNull();
            
            RuleFor(c => c.TestPackage.Name)
                .NotNull()
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(50)
                .MustAsync(async (c, name, token) => !await db.TestPackages
                    .AnyAsync(t => t.Name == name, token))
                .WithMessage("Test package with this name already exists.");

            RuleFor(c => c.TestPackage.Description)
                .MaximumLength(1000);

            // TODO: add rule for contains in test package types list
            RuleFor(c => c.TestPackage.TestPackageType)
                .NotNull()
                .NotEmpty()
                .MaximumLength(50)
                .IsEnumName(typeof(TestPackageType), false);

            RuleFor(c => c.TestPackage.Content)
                .NotNull();
            
            RuleFor(c => c.TestPackage.Content.TestFiles)
                .NotEmpty()
                .NotNull();
            
            RuleForEach(c => c.TestPackage.Content.TestFiles)
                .NotEmpty()
                .NotNull()
                .ChildRules(TestPackageFileRules);

            RuleFor(c => c.TestPackage.Content.Directories)
                .NotNull()
                .NotEmpty();
            
            RuleForEach(c => c.TestPackage.Content.Directories)
                .NotNull()
                .NotEmpty()
                .ChildRules(TestPackageDirectoryRules);
            
            RuleFor(c => c.TestPackage.TestProjectId)
                .NotEmpty()
                .MustAsync(async (c, id, token) => await db.TestProjects
                    .AnyAsync(t => t.Id == id, token))
                .WithMessage("Test project not found.");
        }

        private static void TestPackageDirectoryRules(InlineValidator<NewTestPackageDirectoryDto> validator)
        {
            validator.RuleFor(t => t.DirectoryName)
                .NotNull()
                .NotEmpty();

            validator.RuleForEach(t => t.TestFiles)
                .NotEmpty()
                .NotNull()
                .ChildRules(TestPackageFileRules);
            
            validator.RuleForEach(t => t.Directories)
                .NotNull()
                .NotEmpty()
                .ChildRules(TestPackageDirectoryRules);
        }
        
        private static void TestPackageFileRules(InlineValidator<NewTestPackageFileInfo> validator)
        {
            validator.RuleFor(t => t.Name)
                .NotNull()
                .NotEmpty();

            validator.RuleFor(t => t.Length)
                .Must(l => l < MaxFileSize)
                .WithMessage(t => $"Max file size exceeded. File name: {t.Name} size: {t.Length}.");
        }
    }

    public class Handler(
        ITestsStorageService testsStorageService,
        ITestsManagerContext db,
        IMapper mapper) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken = default)
        {
            var testPackage = mapper.Map<NewTestPackageDto, BeaversTestPackage>(command.TestPackage);
            var testPackageContent = mapper.Map<NewTestPackageContentDto, TestPackageContent>(command.TestPackage.Content);

            var addedTestPackage = await db.TestPackages.AddAsync(testPackage, cancellationToken);
            var testPackageId = addedTestPackage.Entity.Id;
            
            await testsStorageService.AddTestPackageAsync(
                testPackageId,
                testPackageContent,
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