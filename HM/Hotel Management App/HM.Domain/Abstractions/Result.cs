using System.Diagnostics.CodeAnalysis;

namespace HM.Domain.Abstractions;

public class Result
{

    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();
        
        if(isSuccess is false && error == Error.None)
            throw new InvalidOperationException();
        
        IsSuccess = isSuccess;
        Error = error;
    }
    
    #region Properties
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; set; }
    #endregion
    
    public static Result Success() => new Result(true, Error.None);
    public static Result Failure(Error error) => new Result(false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(value, true, Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(default, false, error);
    
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

}