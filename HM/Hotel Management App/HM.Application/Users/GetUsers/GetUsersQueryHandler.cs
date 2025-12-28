using HM.Application.Abstractions.Data;
using HM.Application.Abstractions.Messaging;
using HM.Application.Users.Shared;
using HM.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace HM.Application.Users.GetUsers;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, Result<IReadOnlyList<UserResponse>>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<UserResponse>>> Handle(GetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var usersQuery = _context.Users.AsNoTracking();

        if (request.Filter is not null)
        {
            if (!string.IsNullOrWhiteSpace(request.Filter.SearchTerm))
            {
                usersQuery = usersQuery.Where(u =>
                    u.Name.FirstName.Contains(request.Filter.SearchTerm) ||
                    u.Name.LastName.Contains(request.Filter.SearchTerm) ||
                    u.Contact.Email.Value.Contains(request.Filter.SearchTerm) ||
                    u.Contact.PhoneNumber.Value.Contains(request.Filter.SearchTerm));
            }

            if (request.Filter.Page.HasValue && request.Filter.PageSize.HasValue)
            {
                var skip = (request.Filter.Page.Value - 1) * request.Filter.PageSize.Value;
                usersQuery = usersQuery.Skip(skip).Take(request.Filter.PageSize.Value);
            }
        }

        var users = await usersQuery.ToListAsync(cancellationToken);

        var userResponses = users
            .Select(user => new UserResponse(user))
            .ToList();

        return Result.Success<IReadOnlyList<UserResponse>>(userResponses);
    }
}