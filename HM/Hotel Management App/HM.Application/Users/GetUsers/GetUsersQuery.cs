using HM.Application.Abstractions.Messaging;
using HM.Application.Users.Shared;
using HM.Domain.Abstractions;

namespace HM.Application.Users.GetUsers;

/// <summary>
///     Query to retrieve a filtered list of users.
/// </summary>
/// <param name="Filter">The filter criteria for the query.</param>
public record GetUsersQuery(UserFilter? Filter) : IQuery<Result<IReadOnlyList<UserResponse>>>;