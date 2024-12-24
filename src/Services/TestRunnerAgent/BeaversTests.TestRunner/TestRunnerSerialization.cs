using System.Text;
using Newtonsoft.Json;

namespace BeaversTests.TestRunner;

public static class TestRunnerSerialization
{
    public static byte[] Serialize(object @object)
    {
        var json = JsonConvert.SerializeObject(@object);
        return Encoding.UTF8.GetBytes(json);
    }

    public static TResult DeserializeResult<TResult>(byte[] bytes) where TResult : CommandResult
    {
        var json = Encoding.UTF8.GetString(bytes);
        return JsonConvert.DeserializeObject<TResult>(json)!;
    }
}