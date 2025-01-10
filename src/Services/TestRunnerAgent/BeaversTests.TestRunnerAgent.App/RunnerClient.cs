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
        using HttpClient client = new HttpClient();

        var commandData = command.Serialize();
        
        var response = await client.PostAsync("http://localhost:53999/", new StringContent(commandData));

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsByteArrayAsync();
        var result = TestRunnerSerialization.DeserializeResult<TCommandResult>(responseContent);

        return result;
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}