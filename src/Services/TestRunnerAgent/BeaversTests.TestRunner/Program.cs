using System.Net;
using BeaversTests.TestRunner;

using HttpListener allInterfacesHttpListener = new();
using HttpListener localHttpListener = new();

HttpListener httpListener = allInterfacesHttpListener;

// TODO: Параметризация урла?
allInterfacesHttpListener.Prefixes.Add("http://+:53999/cmd/");

try
{
    allInterfacesHttpListener.Start();
}
catch (HttpListenerException)
{
    allInterfacesHttpListener.Close();
    
    localHttpListener.Prefixes.Clear();
    localHttpListener.Prefixes.Add("http://localhost:53999/cmd/");
    
    localHttpListener.Start();

    httpListener = localHttpListener;
}

Console.WriteLine("Listening...");

while (httpListener.IsListening)
{
    try
    {
        var context = await httpListener.GetContextAsync();

        if (context.Request.HttpMethod != "POST" && 
            !context.Request.IsLocal)
            continue;

        await using var input = context.Request.InputStream;
        using var sr = new StreamReader(input);
        
        var content = await sr.ReadToEndAsync();

        if (!content.Any())
        {
            var responseMessage = "Command is empty."u8.ToArray();
            await context.Response.OutputStream.WriteAsync(responseMessage, 0, responseMessage.Length);
            
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.Close();
            
            continue;
        }

        Console.WriteLine($"Command received. ({content.Length} length)");

        if (!Command.TryDeserialize(content, out var command, out var exception))
        {
            Console.WriteLine(exception);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.Close();
            
            continue;
        }

        Console.WriteLine("Command deserialized. Execution..");
        var result = command!.Execute();

        Console.WriteLine("Command executed. Serialization and response..");
        var responseContent = result.Serialize();
        
        await using var response = context.Response.OutputStream;
        await responseContent.CopyToAsync(response);

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.Close();

        Console.WriteLine("Response sent.");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}