using GTMS.Application.Features.Identity.Auth.Dtos;
using MediatR;

namespace GTMS.Application.Features.Identity.Auth.Commands.Register;

public record RegisterCommand(
    string FirstName, 
    string LastName, 
    string Email, 
    string Password,
    string? StudentNumber
) : IRequest<AuthResponse>;
