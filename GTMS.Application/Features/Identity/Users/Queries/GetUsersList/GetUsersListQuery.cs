using GTMS.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Queries.GetUsersList;

public class GetUsersListQuery : IRequest<List<UserDto>>
{
}

public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, List<UserDto>>
{
    private readonly IGtmsDbContext _context;

    public GetUsersListQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.UserRoles.FirstOrDefault() != null ? u.UserRoles.FirstOrDefault()!.Role.Name : "N/A",
                PhoneNumber = u.PhoneNumber ?? string.Empty,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }
}
