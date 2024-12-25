using System.Net;
using System.Net.Sockets;
using BeaversTests.TestRunner;

Console.WriteLine("Starting...");

using Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

s.Bind(new IPEndPoint(IPAddress.Any, 53999));

s.Listen(10);

while (true)
{
    using var clientSocket = await s.AcceptAsync();

    byte[] buffer = new byte[100000000];
    int bytesRead = await clientSocket.ReceiveAsync(buffer);
    
    if (bytesRead == 0)
        continue;

    var messageBytes = new byte[bytesRead];
    Array.Copy(buffer, messageBytes, bytesRead);

    Console.WriteLine($"Command received. ({bytesRead} bytes)");
    
    if (!Command.TryDeserialize(messageBytes, out var command, out var exception))
    {
        Console.WriteLine(exception);
        
        continue;
    }

    var result = command!.Execute();
    byte[] responseBytes = result.Serialize();
    
    await clientSocket.SendAsync(responseBytes);
}