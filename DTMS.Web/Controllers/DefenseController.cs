using GTMS.Application.Features.Defense.Events.Commands.CreateDefenseEvent;
using GTMS.Application.Features.Academic.AcademicTerms.Queries.GetAcademicTerms; // Assuming this exists or similar
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DTMS.Web.Controllers;

[Authorize]
public class DefenseController : Controller
{
    private readonly IMediator _mediator;

    public DefenseController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Events");
        }
        return View();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Events()
    {
        var events = await _mediator.Send(new GTMS.Application.Features.Defense.Events.Queries.GetDefenseEvents.GetDefenseEventsQuery());
        return View(events); 
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> CreateEvent()
    {
        var terms = await _mediator.Send(new GTMS.Application.Features.Academic.AcademicTerms.Queries.GetAcademicTerms.GetAcademicTermsQuery());
        ViewBag.Terms = new SelectList(terms, "Id", "Name");
        
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEvent(CreateDefenseEventCommand command)
    {
        if (ModelState.IsValid)
        {
            try 
            {
                await _mediator.Send(command);
                return RedirectToAction(nameof(Events));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        
        var terms = await _mediator.Send(new GTMS.Application.Features.Academic.AcademicTerms.Queries.GetAcademicTerms.GetAcademicTermsQuery());
        ViewBag.Terms = new SelectList(terms, "Id", "Name");
        
        return View(command);
    }
}
