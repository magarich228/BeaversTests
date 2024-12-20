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

    public byte[] Serialize()
    {
        var json = JsonConvert.SerializeObject(this);
        return Encoding.UTF8.GetBytes(json);
    }
    
    public static bool TryDeserialize(byte[] bytes, out Command? command, out Exception? exception)
    {
        try
        {
            var json = Encoding.UTF8.GetString(bytes);
            
            var info = JsonConvert.DeserializeObject<CommandInfoInternal>(json);
            
            if (info is null)
                throw new TestRunnerException("Failed to deserialize command");
            
            var type = Type.GetType(info.TypeFullName, true, true);
            
            command = (Command?)JsonConvert.DeserializeObject(json, type!);
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