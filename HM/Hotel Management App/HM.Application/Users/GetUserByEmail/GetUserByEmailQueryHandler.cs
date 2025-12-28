using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Users.GetUsers;
using HM.Domain.Abstractions;
using HM.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Users.GetUserByEmail;

internal sealed class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, Result<UserResponse>>
{
    private readonly IApplicationDbContext _context;

    public GetUserByEmailQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UserResponse>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        if (!request.Email.Contains('@')) return Result.Failure<UserResponse>(UserErrors.InvalidEmail);

        var parts = request.Email.Split('@');
        var localPart = parts[0];
        var domainPart = parts[1];

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Contact.Email.Value == localPart && u.Contact.Email.Domain == domainPart,
                cancellationToken);

        if (user is null) return Result.Failure<UserResponse>(UserErrors.NotFound);

        return Result.Success(new UserResponse(user));
    }
}