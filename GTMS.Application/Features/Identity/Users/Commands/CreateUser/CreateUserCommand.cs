using MediatR;

namespace GTMS.Application.Features.Identity.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string RoleName,
    string? PhoneNumber,
    Guid? DepartmentId,
    Guid? ProgramId,
    string? StudentNumber
) : IRequest<Guid>;
