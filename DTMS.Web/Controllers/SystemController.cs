using GTMS.Application.Features.System.AccessLogs.Queries.GetAccessLogs;
using GTMS.Application.Features.System.Configuration.Queries.GetSystemSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SystemController : Controller
{
    private readonly IMediator _mediator;

    public SystemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Configuration()
    {
        return View(await _mediator.Send(new GetSystemSettingsQuery()));
    }

    [HttpGet]
    public async Task<IActionResult> AccessLogs()
    {
        return View(await _mediator.Send(new GetAccessLogsQuery()));
    }
}
