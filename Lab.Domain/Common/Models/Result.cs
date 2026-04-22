namespace Lab.Domain.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public IEnumerable<string> Errors { get; }


    public static Result Success()
    {
        return new Result(true, []);
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

    public static Result Failure(string error)
    {
        return new Result(false, [error]);
    }
}

public class Result<TValue>
{
    internal Result(bool succeeded, TValue value, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public IEnumerable<string> Errors { get; }
    public TValue Value { get; }

    public static Result<TValue> Success(TValue value)
    {
        return new Result<TValue>(true, value, []);
    }

    public static Result<TValue> Failure(IEnumerable<string> errors)
    {
        return new Result<TValue>(false, default, errors);
    }

    public static Result<TValue> Failure(string error)
    {
        return new Result<TValue>(false, default, [error]);
    }
}

