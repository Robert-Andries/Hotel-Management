namespace HM.Domain.Abstractions;

/// <summary>
///     Represents a domain error with a code and a description.
/// </summary>
/// <param name="Code">The unique error code.</param>
/// <param name="Name">The error description.</param>
public record Error(string Code, string Name)
{
    /// <summary>
    ///     Represents no error.
    /// </summary>
    public static Error None = new(string.Empty, string.Empty);

    /// <summary>
    ///     Represents a null value error.
    /// </summary>
    public static Error NullValue = new("Error.NullValue", "Null value was provided");

    /// <summary>
    ///     Represents an operation cancellation error.
    /// </summary>
    public static Error OperationCanceled = new("Error.OperationCanceled", "Operation was canceled");
}