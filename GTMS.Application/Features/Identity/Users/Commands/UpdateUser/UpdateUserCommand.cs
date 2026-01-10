using MediatR;

namespace GTMS.Application.Features.Identity.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? PhoneNumber,
    Guid? DepartmentId,
    Guid? ProgramId,
    string? StudentNumber
) : IRequest;
