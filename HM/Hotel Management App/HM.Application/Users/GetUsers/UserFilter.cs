namespace HM.Application.Users.GetUsers;

/// <summary>
///     Represents criteria for filtering users.
/// </summary>
/// <param name="SearchTerm">Search term for name/email/phone.</param>
/// <param name="Page">Optional page number for pagination.</param>
/// <param name="PageSize">Optional page size for pagination.</param>
public record UserFilter(
    string SearchTerm,
    int? Page,
    int? PageSize);