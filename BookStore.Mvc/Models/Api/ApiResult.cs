namespace BookStore.Mvc.Models.Api;

public class ApiResult<T>
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int StatusCode { get; set; }

    public static ApiResult<T> Success(T? data, string message = "", int statusCode = 200)
    {
        return new ApiResult<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ApiResult<T> Failure(string message, int statusCode)
    {
        return new ApiResult<T>
        {
            IsSuccess = false,
            Message = message,
            StatusCode = statusCode
        };
    }
}
