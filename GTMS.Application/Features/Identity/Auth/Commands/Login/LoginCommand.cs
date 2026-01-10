using GTMS.Application.Features.Identity.Auth.Dtos;
using MediatR;

namespace GTMS.Application.Features.Identity.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
