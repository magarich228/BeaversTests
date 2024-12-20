using System.Net.Sockets;
using System.Text;
using BeaversTests.TestRunner;

namespace BeaversTests.TestRunnerAgent.App;

public class RunnerClient : IDisposable
{
    private readonly Socket _socket = 
        new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    public async Task SendAsync(Command command)
    {
        await _socket.ConnectAsync("127.0.0.1", 53999);

        var messageBytes = command.Serialize();
        _socket.Send(messageBytes);

        // Чтение ответа от сервера
        byte[] buffer = new byte[1024];
        int bytesRead = _socket.Receive(buffer);
        string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Получен ответ от сервера: {response}");
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}