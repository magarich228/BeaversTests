using Newtonsoft.Json;

namespace BeaversTests.Platform.Public
{
    public class Result
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        
        [JsonProperty("error")]
        public string? Error { get; set; }
        
        public static Result MakeSuccess()
        {
            return new Result()
            {
                Success = true
            };
        }

        public static Result MakeFailure(string? error)
        {
            return new Result()
            {
                Success = false,
                Error = error
            };
        }
    }

    public class Result<T> : Result
    {
        [JsonProperty("data")]
        public T? Data { get; set; }

        public static Result<T> MakeSuccess(T? data)
        {
            return new Result<T>()
            {
                Success = true,
                Data = data
            };
        }

        public static Result<T> MakeFailure(T? data, string error)
        {
            return new Result<T>()
            {
                Success = false,
                Data = data,
                Error = error
            };
        }
    }
}