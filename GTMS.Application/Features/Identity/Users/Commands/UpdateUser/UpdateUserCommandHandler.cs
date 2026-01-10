using GTMS.Application.Common.Interfaces;
using GTMS.Application.Common.Exceptions;
using GTMS.Domain.Entities.Academic;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IGtmsDbContext _context;

    public UpdateUserCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException($"User with ID {request.UserId} not found.");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PhoneNumber = request.PhoneNumber ?? string.Empty;
        user.UpdatedAt = DateTime.UtcNow;

        var roleName = user.UserRoles.FirstOrDefault()?.Role.Name;

        if (roleName == "Student")
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id, cancellationToken);
            if (student != null)
            {
                if (request.DepartmentId.HasValue) student.DepartmentId = request.DepartmentId.Value;
                if (request.ProgramId.HasValue) student.ProgramId = request.ProgramId.Value;
                if (!string.IsNullOrEmpty(request.StudentNumber)) student.StudentNumber = request.StudentNumber;
                student.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                 if (request.DepartmentId != null && request.ProgramId != null && !string.IsNullOrWhiteSpace(request.StudentNumber))
                {
                    var newStudent = new Student
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        DepartmentId = request.DepartmentId.Value,
                        ProgramId = request.ProgramId.Value,
                        StudentNumber = request.StudentNumber,
                        EnrollmentYear = DateTime.UtcNow.Year,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Students.Add(newStudent);
                }
            }
        }
        else if (roleName == "Advisor")
        {
            var advisor = await _context.Advisors.FirstOrDefaultAsync(a => a.UserId == user.Id, cancellationToken);
            if (advisor != null)
            {
                if (request.DepartmentId.HasValue) advisor.DepartmentId = request.DepartmentId.Value;
                advisor.UpdatedAt = DateTime.UtcNow;
            }
             else
            {
                 if (request.DepartmentId != null)
                {
                    var newAdvisor = new Advisor
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        DepartmentId = request.DepartmentId.Value,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Advisors.Add(newAdvisor);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
