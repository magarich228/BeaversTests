using AutoMapper;
using BeaversTests.Common.CQRS;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Commands;
using BeaversTests.TestsManager.App.Abstractions;
using BeaversTests.TestsManager.App.Dtos;
using BeaversTests.TestsManager.Core.TestPackage;
using BeaversTests.TestsManager.Events.TestPackage;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
        private readonly IConfiguration _configuration;
        
        public Validator(
            ITestsManagerContext db,
            IConfiguration configuration)
        {
            _configuration = configuration;
            
            // TODO: test package content validator;
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

            RuleFor(c => c.TestPackage.TestDriver)
                .NotNull()
                .NotEmpty()
                .MaximumLength(25)
                .MustAsync(async (c, driver, token) => await db.TestDrivers
                    .AnyAsync(t => t.Key == driver, token));

            // TODO: Подробные сообщнения об ошибках валидации
            RuleFor(c => c.TestPackage.Content)
                .NotNull()
                .MustAsync(async (t, content, token) => await IsContentValidAsync(content, token))
                .WithMessage("Test package content is not valid.");

            RuleFor(c => c.TestPackage.TestProjectId)
                .NotEmpty()
                .MustAsync(async (c, id, token) => await db.TestProjects
                    .AnyAsync(t => t.Id == id, token))
                .WithMessage("Test project not found.");
        }

        private async Task<bool> IsContentValidAsync(
            NewTestPackageContentDto content,
            CancellationToken cancellationToken = default)
        {
            var isValid = true;

            var context = new ContentValidationContext()
            {
                FileValidator = new FileValidator(_configuration),
                DirectoryValidator = new DirectoryValidator()
            };

            isValid |= await IsContentFilesValidAsync(context, content.TestFiles, cancellationToken);

            foreach (var directory in content.Directories)
            {
                isValid |= await IsContentDirectoryValidAsync(context, directory, cancellationToken);
            }

            return isValid;
        }

        private async Task<bool> IsContentDirectoryValidAsync(
            ContentValidationContext context,
            NewTestPackageDirectoryInfo directory,
            CancellationToken cancellationToken = default) =>
             (await context.DirectoryValidator.ValidateAsync(directory, cancellationToken)).IsValid &&
                   await IsContentFilesValidAsync(context, directory.TestFiles, cancellationToken) &&
                   directory.Directories.All(d =>
                       IsContentDirectoryValidAsync(context, d, cancellationToken).Result);
        

        private Task<bool> IsContentFilesValidAsync(
            ContentValidationContext context,
            IEnumerable<NewTestPackageFileInfo> files,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(files.All(f => context.FileValidator.ValidateAsync(f, cancellationToken).Result.IsValid));


        private class FileValidator : AbstractValidator<NewTestPackageFileInfo>
        {
            private const string TestPackageMaxFileMbSizeConfigurationKey = "TestPackageMaxFileMbSize";
            
            public FileValidator(IConfiguration configuration)
            {
                var maxFileSizeInBytes = configuration.GetValue<long>(TestPackageMaxFileMbSizeConfigurationKey) * 1024 * 1024;
                
                RuleFor(t => t.Name)
                    .NotNull()
                    .NotEmpty();

                RuleFor(t => t.Length)
                    .Must(l => l < maxFileSizeInBytes)
                    .WithMessage(t => $"Max file size exceeded. File name: {t.Name} size: {t.Length}.");

                RuleFor(t => t.Content)
                    .NotNull();
            }
        }

        private class DirectoryValidator : AbstractValidator<NewTestPackageDirectoryInfo>
        {
            public DirectoryValidator()
            {
                RuleFor(t => t.DirectoryName)
                    .NotNull()
                    .NotEmpty();
            }
        }

        private class ContentValidationContext
        {
            public required DirectoryValidator DirectoryValidator { get; init; }
            public required FileValidator FileValidator { get; init; }
        }
    }

    public class Handler(
        IEventStore eventStore,
        ITestsStorageService testsStorageService,
        IMapper mapper) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command command, CancellationToken cancellationToken = default)
        {
            var testPackage = new TestPackageAggregate();
            var @event = new TestPackageAddedEvent()
            {
                Id = Guid.NewGuid(),
                Name = command.TestPackage.Name,
                Description = command.TestPackage.Description,
                TestDriverKey = command.TestPackage.TestDriver,
                TestProjectId = command.TestPackage.TestProjectId,
            };
            
            var testPackageContent =
                 mapper.Map<NewTestPackageContentDto, TestPackageContent>(command.TestPackage.Content);

            await testsStorageService.AddTestPackageAsync(@event.Id, testPackageContent, cancellationToken);
            
            testPackage.ApplyCreated(@event);

            await eventStore.StoreAsync(testPackage, cancellationToken);

            return new Result()
            {
                TestPackageId = testPackage.Id
            };
        }
    }
}