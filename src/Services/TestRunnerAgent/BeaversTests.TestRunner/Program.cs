using System.Net;
using System.Net.Sockets;
using BeaversTests.TestRunner;


// using Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//
// s.Bind(new IPEndPoint(IPAddress.Any, 53999));
//
// s.Listen(10);

using HttpListener httpListener = new();

httpListener.Prefixes.Add("http://localhost:53999/");
httpListener.Start();

Console.WriteLine("Starting...");

while (true)
{
    try
    {
        var context = httpListener.GetContext();

        if (context.Request.HttpMethod != "POST" &&
            !context.Request.IsLocal)
            continue;

        await using var input = context.Request.InputStream;
        using var sr = new StreamReader(input);
        
        var content = await sr.ReadToEndAsync();

        if (!content.Any())
            continue;

        Console.WriteLine($"Command received. ({content.Length} length)");

        if (!Command.TryDeserialize(content, out var command, out var exception))
        {
            Console.WriteLine(exception);

            continue;
        }

        var result = command!.Execute();
        var responseContent = result.Serialize();

        await using var response = context.Response.OutputStream;
        await responseContent.CopyToAsync(response);

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}