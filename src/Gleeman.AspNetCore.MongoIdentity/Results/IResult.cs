namespace Gleeman.AspNetCore.MongoIdentity.Results;

public interface IResult
{
    bool IsSuccess { get; set;}
    string? Message { get; set; }
    List<string>? Errors { get; set; }
}

public interface IResult<T> : IResult
{
    T? Value { get; set; }
}
