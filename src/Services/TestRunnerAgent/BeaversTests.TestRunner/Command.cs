using System.Text;
using Newtonsoft.Json;

namespace BeaversTests.TestRunner;

public abstract class Command
{
    public string TypeFullName { get; }

    protected Command()
    {
        TypeFullName = GetType().FullName!;
    }

    public string Serialize()
    {
        var json = JsonConvert.SerializeObject(this);
        return json;
    }
    
    internal abstract CommandResult Execute();
    
    internal static bool TryDeserialize(string json, out Command? command, out Exception? exception)
    {
        try
        {
            Console.WriteLine(json);
            
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