using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Common.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Queries.GetUserForEdit;

public class GetUserForEditQuery : IRequest<EditUserVm?>
{
    public Guid UserId { get; set; }

    public GetUserForEditQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class EditUserVm
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public Guid? DepartmentId { get; set; }
    public Guid? ProgramId { get; set; }
    public string? StudentNumber { get; set; }

    // Dropdown Data
    public List<DepartmentDto> Departments { get; set; } = new();
    public List<ProgramDto> Programs { get; set; } = new();
}

public class GetUserForEditQueryHandler : IRequestHandler<GetUserForEditQuery, EditUserVm?>
{
    private readonly IGtmsDbContext _context;

    public GetUserForEditQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<EditUserVm?> Handle(GetUserForEditQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null) return null; // Or throw NotFoundException

        var vm = new EditUserVm
        {
            UserId = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.UserRoles.FirstOrDefault()?.Role.Name ?? "Student",
            Departments = await _context.Departments.Select(d => new DepartmentDto { Id = d.Id, Name = d.Name }).ToListAsync(cancellationToken),
            Programs = await _context.Programs.Select(p => new ProgramDto { Id = p.Id, DepartmentId = p.DepartmentId, Name = p.Name }).ToListAsync(cancellationToken)
        };

        if (vm.Role == "Student")
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id, cancellationToken);
            if (student != null)
            {
                vm.DepartmentId = student.DepartmentId;
                vm.ProgramId = student.ProgramId;
                vm.StudentNumber = student.StudentNumber;
            }
        }
        else if (vm.Role == "Advisor")
        {
            var advisor = await _context.Advisors.FirstOrDefaultAsync(a => a.UserId == user.Id, cancellationToken);
            if (advisor != null)
            {
                vm.DepartmentId = advisor.DepartmentId;
            }
        }

        return vm;
    }
}
