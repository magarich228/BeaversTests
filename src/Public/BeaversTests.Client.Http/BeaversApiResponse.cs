using BeaversTests.Auth.Public;

namespace BeaversTests.Client.Http;

public class BeaversApiResponse<T> : OperationResult
{
    public T? Data { get; set; }
    public int StatusCode { get; set; }
}