namespace BeaversTests.Common.CQRS.Commands;

public interface ICommandBus
{
    Task<TResponse> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default(CancellationToken));

    Task SendAsync(ICommand command, CancellationToken cancellationToken = default(CancellationToken));
}