using HM.Application.Abstractions.Messaging;
using HM.Application.Users.Shared;
using HM.Domain.Abstractions;

namespace HM.Application.Users.GetUsers;

public record GetUsersQuery(UserFilter? Filter) : IQuery<Result<IReadOnlyList<UserResponse>>>;