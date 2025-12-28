using HM.Application.Abstractions.Messaging;
using HM.Application.Users.GetUsers;
using HM.Domain.Abstractions;

namespace HM.Application.Users.GetUserByEmail;

public record GetUserByEmailQuery(string Email) : IQuery<Result<UserResponse>>;