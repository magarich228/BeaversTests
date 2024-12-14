using AutoMapper;
using BeaversTests.Common.CQRS.Abstractions;
using BeaversTests.Common.CQRS.Events;
using BeaversTests.TestRunnerAgent.Events;
using BeaversTests.TestRunnerController.Events;
using BeaversTests.TestsManager.Core;
using BeaversTests.TestsManager.Core.TestDriver;
using BeaversTests.TestsManager.Events.TestDriver;
using Microsoft.Extensions.Logging;

namespace BeaversTests.TestsManager.App.EventHandlers;

public class TestDriverValidationStatusEventHandler(
    IEventStore eventStore,
    IMapper mapper,
    ILogger<TestDriverValidationStatusEventHandler> logger) : 
    IEventHandler<TestDriverValidationResultEvent>,
    IEventHandler<TestDriverValidationIsNotPossibleEvent>
{
    public async Task Handle(TestDriverValidationResultEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test driver {TestDriverKey} {AgId} validation result event has been received.", 
            notification.Key, 
            notification.AgId);
        
        var testDriverAggregate = await eventStore.AggregateStreamAsync<TestDriverAggregate>(
            new AggregateInfo()
            {
                Id = notification.AgId
            }, cancellationToken);

        var @event = mapper.Map<TestDriverValidationStatusEvent>(notification);
        testDriverAggregate.ApplyValidationStatus(@event);

        await eventStore.StoreAsync(testDriverAggregate, cancellationToken);
    }

    public async Task Handle(TestDriverValidationIsNotPossibleEvent notification, CancellationToken cancellationToken)
    {
        logger.LogDebug("Test driver {TestDriverKey} {AgId} validation is not possible event has been received.",
            notification.Key,
            notification.AgId);
        
        var testDriverAggregate = await eventStore.AggregateStreamAsync<TestDriverAggregate>(
            new AggregateInfo()
            {
                Id = notification.AgId
            }, cancellationToken);

        var @event = new TestDriverValidationStatusEvent()
        {
            Key = notification.Key,
            AgId = notification.AgId,
            ValidationStatus  = ValidationResult.Unknown.ToString(),
            ValidationMessage = "Validation is not possible because test agent is not available."
        };
        
        testDriverAggregate.ApplyValidationStatus(@event);
        
        await eventStore.StoreAsync(testDriverAggregate, cancellationToken);
    }
}