using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Identity.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IGtmsDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IGtmsDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure("Email", $"Email '{request.Email}' is already taken.")
            });
        }

        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == request.RoleName, cancellationToken);
        if (role == null)
        {
             throw new NotFoundException($"Role '{request.RoleName}' not found.");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        user.UserRoles = new List<UserRole>
        {
            new UserRole
            {
                Id = Guid.NewGuid(),
                RoleId = role.Id,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        
        _context.Users.Add(user);

        if (request.RoleName == "Student")
        {
            if (request.DepartmentId == null || request.ProgramId == null || string.IsNullOrWhiteSpace(request.StudentNumber))
            {
                throw new ValidationException(new[] { new FluentValidation.Results.ValidationFailure("Student Details", "Department, Program, and Student Number are required for Students.") });
            }

            var student = new GTMS.Domain.Entities.Academic.Student
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
            _context.Students.Add(student);
        }
        else if (request.RoleName == "Advisor")
        {
            if (request.DepartmentId == null)
            {
               throw new ValidationException(new[] 
            { 
                 new FluentValidation.Results.ValidationFailure("Role", "Role not found.") 
            });
            }

            var advisor = new GTMS.Domain.Entities.Academic.Advisor
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DepartmentId = request.DepartmentId.Value,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.Advisors.Add(advisor);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
