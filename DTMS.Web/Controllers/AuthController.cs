using GTMS.Application.Features.Identity.Auth.Commands.Login;
using GTMS.Application.Features.Identity.Auth.Commands.Register;
using GTMS.Application.Features.Identity.Auth.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command)
    {
        return await _mediator.Send(command);
    }
}
