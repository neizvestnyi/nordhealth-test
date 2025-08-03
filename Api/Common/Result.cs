namespace Api.Common;

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public ErrorTypeEnum? ErrorType { get; private set; }

    protected Result(bool isSuccess, string? errorMessage = null, ErrorTypeEnum? errorType = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
    }

    public static Result Success() => new(true);
    public static Result Failure(string errorMessage, ErrorTypeEnum errorType) => new(false, errorMessage, errorType);
}

public class Result<T> : Result
{
    public T? Data { get; private set; }

    private Result(bool isSuccess, T? data, string? errorMessage = null, ErrorTypeEnum? errorType = null) 
        : base(isSuccess, errorMessage, errorType)
    {
        Data = data;
    }

    public static Result<T> Success(T data) => new(true, data);
    public new static Result<T> Failure(string errorMessage, ErrorTypeEnum errorType) => new(false, default, errorMessage, errorType);
}