namespace HM.Domain.Abstractions;

public record Error(string Code, string Name)
{
    public static Error None = new(string.Empty, string.Empty);
    public static Error NullValue = new("Error.NullValue", "Null value was provided");
    public static Error OperationCanceled = new("Error.OperationCanceled", "Operation was canceled");
}