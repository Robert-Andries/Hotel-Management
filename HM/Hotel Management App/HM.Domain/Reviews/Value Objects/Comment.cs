namespace HM.Domain.Reviews.Value_Objects;

/// <summary>
///     Represents the text content of a review.
/// </summary>
/// <param name="Title">The review title or summary.</param>
/// <param name="Content">The full body of the review.</param>
public record Comment(string Title, string Content);