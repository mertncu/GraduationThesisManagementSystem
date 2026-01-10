using MediatR;

namespace GTMS.Application.Features.Identity.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : IRequest;
