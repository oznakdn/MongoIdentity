
namespace Gleeman.AspNetCore.MongoIdentity.Results;

public class Result : IResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }

    public static Result Success(string message = null)
    {
        return new Result
        {
            IsSuccess = true,
            Message = message
        };
    }

    public static Result Failure(string message = null, List<string> errors = null)
    {
        return new Result
        {
            IsSuccess = false,
            Message = message,
            Errors = errors
        };
    }
}

public class Result<T> : IResult<T>
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
    public T? Value { get ; set; }

    public static Result<T> Success(T data, string message = null)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Value = data,
            Message = message
        };
    }

    public static Result<T> Failure(string message = null, List<string> errors = null)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors
        };
    }


}
