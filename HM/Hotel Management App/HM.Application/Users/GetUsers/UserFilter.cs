namespace HM.Application.Users.GetUsers;

public record UserFilter(
    string? SearchTerm,
    int? Page,
    int? PageSize);
