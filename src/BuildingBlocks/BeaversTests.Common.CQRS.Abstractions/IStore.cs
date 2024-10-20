﻿namespace BeaversTests.Common.CQRS.Abstractions;

public interface IStore : IAsyncDisposable
{
    Task AddAsync(
        StreamState stream, 
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<StreamState>> GetEventsAsync(
        AggregateInfo info, 
        CancellationToken cancellationToken = default);
}