using GTMS.Application.Features.Thesis.ThesisMilestones.Commands.CreateThesisMilestone;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Authorize]
public class ThesisMilestonesController : Controller
{
    private readonly IMediator _mediator;

    public ThesisMilestonesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateThesisMilestoneCommand command)
    {
        try 
        {
            await _mediator.Send(command);
        }
        catch(Exception ex) 
        {
            // Ideally use TempData for errors
        }

        return RedirectToAction("Details", "ThesisProjects", new { id = command.ThesisId });
    }
}
