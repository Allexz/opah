namespace AccountingOffice.Domain.Core.Common;

public class DomainResult<T>
{
    public T Value { get; }
    public string Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private DomainResult(T value)
    {
        Value = value;
        Error = string.Empty;
        IsSuccess = true;
    }

    private DomainResult(string error)
    {
        Value = default!;
        Error = error ?? "Erro desconhecido";
        IsSuccess = false;
    }

    public static DomainResult<T> Success(T value) => new DomainResult<T>(value);
    public static DomainResult<T> Failure(string error) => new DomainResult<T>(error);

    // Métodos úteis para functional programming
    public DomainResult<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return IsSuccess
            ? DomainResult<TResult>.Success(mapper(Value))
            : DomainResult<TResult>.Failure(Error);
    }

    public DomainResult<TResult> Bind<TResult>(Func<T, DomainResult<TResult>> binder)
    {
        return IsSuccess
            ? binder(Value)
            : DomainResult<TResult>.Failure(Error);
    }

    public T ValueOr(T defaultValue) => IsSuccess ? Value : defaultValue;
}

public class DomainResult
{
    public string Error { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private DomainResult(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
    }

    public static DomainResult Success() => new DomainResult(true, string.Empty);
    public static DomainResult Failure(string error) => new DomainResult(false, error);

    public static DomainResult Combine(params DomainResult[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return Failure(result.Error);
        }
        return Success();
    }
}