using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Common.Dtos;
using GTMS.Application.Features.Identity.Users.Dtos;
using GTMS.Application.Features.Identity.Users.Queries.GetCreateUserFormData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Queries.GetCreateUserPage;

public record GetCreateUserPageQuery : IRequest<CreateUserPageVm>;

public class GetCreateUserPageQueryHandler : IRequestHandler<GetCreateUserPageQuery, CreateUserPageVm>
{
    private readonly IGtmsDbContext _context;

    public GetCreateUserPageQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<CreateUserPageVm> Handle(GetCreateUserPageQuery request, CancellationToken cancellationToken)
    {
        var departments = await _context.Departments
            .AsNoTracking()
            .Select(d => new SelectListItemDto { Id = d.Id, Name = d.Name })
            .ToListAsync(cancellationToken);

        var programs = await _context.Programs
            .AsNoTracking()
            .Select(p => new SelectListItemDto { Id = p.Id, Name = p.Name, DepartmentId = p.DepartmentId })
            .ToListAsync(cancellationToken);

        return new CreateUserPageVm
        {
            Departments = departments,
            Programs = programs // Ideally we filter by selected department in UI, but here we return all so JS can filter.
        };
    }
}
