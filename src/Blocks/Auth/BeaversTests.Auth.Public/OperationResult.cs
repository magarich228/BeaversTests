using Newtonsoft.Json;

namespace BeaversTests.Auth.Public;

public abstract class OperationResult
{
    [JsonProperty("success")]
    public bool Success { get; set; }
    
    [JsonProperty("error")]
    public string? Error { get; set; }
}