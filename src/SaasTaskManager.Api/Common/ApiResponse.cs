namespace SaasTaskManager.Api.Common;

public class ApiResponse<T>
{
    public bool Succeeded { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public IEnumerable<string> Errors { get; set; }

    public static ApiResponse<T> Success(T data, string message = null)
    {
        return new ApiResponse<T>
        {
            Succeeded = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Failure(string message, IEnumerable<string> errors = null)
    {
        return new ApiResponse<T>
        {
            Succeeded = false,
            Message = message,
            Errors = errors
        };
    }
}


// Optional: For responses without data
public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Success(string message = null)
    {
        return new ApiResponse { Succeeded = true, Message = message };
    }

    public static ApiResponse Failure(string message, IEnumerable<string> errors = null)
    {
        return new ApiResponse { Succeeded = false, Message = message, Errors = errors };
    }
}
