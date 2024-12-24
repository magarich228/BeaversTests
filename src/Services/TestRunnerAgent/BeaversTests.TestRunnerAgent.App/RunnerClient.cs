using System.Net.Sockets;
using BeaversTests.TestRunner;

namespace BeaversTests.TestRunnerAgent.App;

public class RunnerClient : IDisposable
{
    private readonly Socket _socket = 
        new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public async Task<TCommandResult> SendAsync<TCommandResult>(Command command) 
        where TCommandResult : CommandResult
    {
        await _socket.ConnectAsync("127.0.0.1", 53999);

        var messageBytes = command.Serialize();
        _socket.Send(messageBytes);

        byte[] buffer = new byte[1024];
        int bytesRead = _socket.Receive(buffer);

        var result = TestRunnerSerialization.DeserializeResult<TCommandResult>(buffer);

        return result;
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}