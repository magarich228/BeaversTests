using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerController.Events;
using BeaversTests.TestsManager.Core;
using BeaversTests.TestsManager.Core.TestPackage;
using BeaversTests.TestsManager.Events.TestPackage;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.EventHandlers;

public class TestPackageValidationStatusEventsHandler(
    IEventStore eventStore,
    ILogger<TestPackageValidationStatusEventsHandler> logger) : IEventHandler<TestPackageValidationIsNotPossibleEvent>
{
    public async Task Handle(TestPackageValidationIsNotPossibleEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Test package {TestPackageId} validation is not possible event has been received.", notification.Id);
        
        var testPackageAggregate = await eventStore.AggregateStreamAsync<TestPackageAggregate>(
            new AggregateInfo()
            {
                Id = notification.Id
            }, cancellationToken);

        var validationStatusEvent = new TestPackageValidationStatusEvent()
        {
            Id = notification.Id,
            ValidationStatus = ValidationResult.Unknown.ToString(),
            ValidationMessage = "Test package validation is not possible. Test agents pool is empty."
        };
        
        testPackageAggregate.ApplyValidationResult(validationStatusEvent);

        await eventStore.StoreAsync(testPackageAggregate, cancellationToken);
    }
}