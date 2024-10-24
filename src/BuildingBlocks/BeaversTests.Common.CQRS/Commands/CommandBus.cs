﻿using MediatR;

namespace BeaversTests.Common.CQRS.Commands;

public class CommandBus(IMediator mediator) : ICommandBus
{
    private readonly IMediator _mediator = mediator ?? throw new Exception($"Missing dependency '{nameof(IMediator)}'");

    public virtual async Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result;
    }

    public virtual async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(command, cancellationToken);
    }
}