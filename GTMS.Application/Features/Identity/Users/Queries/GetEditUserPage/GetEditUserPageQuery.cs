using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Common.Dtos;
using GTMS.Application.Features.Identity.Users.Dtos;
using GTMS.Application.Features.Identity.Users.Queries.GetCreateUserFormData;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Queries.GetEditUserPage;

public record GetEditUserPageQuery(Guid UserId) : IRequest<EditUserPageVm>;

public class GetEditUserPageQueryHandler : IRequestHandler<GetEditUserPageQuery, EditUserPageVm>
{
    private readonly IGtmsDbContext _context;

    public GetEditUserPageQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<EditUserPageVm> Handle(GetEditUserPageQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { request.UserId }, cancellationToken);
        if (user == null) throw new NotFoundException(nameof(GTMS.Domain.Entities.Identity.User), request.UserId);

        var roles = await _context.UserRoles
            .Where(ur => ur.UserId == request.UserId)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync(cancellationToken);
        
        string userRole = roles.FirstOrDefault() ?? "Student";

        // Fetch additional details if student or advisor
        Guid? departmentId = null;
        Guid? programId = null;
        string? studentNumber = null;

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == request.UserId, cancellationToken);
        if (student != null)
        {
            departmentId = student.DepartmentId;
            programId = student.ProgramId;
            studentNumber = student.StudentNumber;
        }
        else
        {
            var advisor = await _context.Advisors.FirstOrDefaultAsync(a => a.UserId == request.UserId, cancellationToken);
            if (advisor != null)
            {
                departmentId = advisor.DepartmentId;
            }
        }

        var departments = await _context.Departments
            .AsNoTracking()
            .Select(d => new SelectListItemDto { Id = d.Id, Name = d.Name })
            .ToListAsync(cancellationToken);

        // Fetch ALL programs for simpler client-side filtering or just fetch relevant.
        // For Edit page, usually we want to see programs of the selected department.
        var programs = await _context.Programs
             .AsNoTracking()
             //.Where(p => p.DepartmentId == departmentId) // Maybe all so we can change department?
             .Select(p => new SelectListItemDto { Id = p.Id, Name = p.Name, DepartmentId = p.DepartmentId })
             .ToListAsync(cancellationToken);

        return new EditUserPageVm
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = userRole,
            DepartmentId = departmentId,
            ProgramId = programId,
            StudentNumber = studentNumber,
            Departments = departments,
            Programs = programs
        };
    }
}
