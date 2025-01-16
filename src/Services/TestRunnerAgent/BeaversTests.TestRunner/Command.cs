using Newtonsoft.Json;

namespace BeaversTests.TestRunner;

public abstract class Command
{
    public string TypeFullName { get; }

    protected Command()
    {
        TypeFullName = GetType().FullName!;
    }

    public async Task<TCommandResult> SendAsync<TCommandResult>() 
        where TCommandResult : CommandResult
    {
        using HttpClient client = new HttpClient();

        var commandData = TestRunnerSerialization.Serialize(this);
        var content = new StreamContent(commandData);
        
        using var response = await client.PostAsync("http://localhost:53999/cmd/", content);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsByteArrayAsync();
        var result = TestRunnerSerialization.DeserializeResult<TCommandResult>(responseContent);

        return result;
    }
    
    internal abstract CommandResult Execute();
    
    internal static bool TryDeserialize(string json, out Command? command, out Exception? exception)
    {
        try
        {
            var info = JsonConvert.DeserializeObject<CommandInfoInternal>(json);
            
            if (info is null)
                throw new TestRunnerException("Failed to deserialize command");
            
            var type = Type.GetType(info.TypeFullName, true, true);
            
            command = (Command?)JsonConvert.DeserializeObject(json, type!) ??
                      throw new TestRunnerException("Failed to deserialize command");
            exception = null;
            
            return true;
        }
        catch (Exception ex)
        {
            command = null;
            exception = new TestRunnerException(null, ex);
            
            return false;
        }
    }

    class CommandInfoInternal
    {
        public string TypeFullName { get; set; } = null!;
    }
}