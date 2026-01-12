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

    [Authorize(Roles = "Advisor,Admin")]
    [HttpGet]
    public async Task<IActionResult> JuryManagement(Guid thesisId)
    {
        var model = await _mediator.Send(new GTMS.Application.Features.Defense.Jury.Queries.GetDefenseJury.GetDefenseJuryQuery { ThesisId = thesisId });
        return View(model);
    }

    [Authorize(Roles = "Advisor,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddJuryMember(GTMS.Application.Features.Defense.Jury.Commands.AddJuryMember.AddJuryMemberCommand command)
    {
        try 
        {
            await _mediator.Send(command);
            TempData["Success"] = "Jury member added successfully.";
        }
        catch (Exception ex)
        {
             TempData["Error"] = ex.Message;
        }
        
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [Authorize(Roles = "Advisor,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveJuryMember(Guid id)
    {
        try
        {
            await _mediator.Send(new GTMS.Application.Features.Defense.Jury.Commands.RemoveJuryMember.RemoveJuryMemberCommand { JuryMemberId = id });
            TempData["Success"] = "Member removed successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [Authorize(Roles = "Advisor,Admin")]
    [HttpGet]
    public async Task<IActionResult> EnterScore(Guid thesisId)
    {
        // Reuse GetDefenseJuryQuery to get session details, or create new query.
        // For simplicity, let's just get the session ID via query
        var model = new GTMS.Application.Features.Defense.Grading.Commands.EnterDefenseScore.EnterDefenseScoreCommand();
        
        // We need to fetch the session ID for this thesis.
        // Let's use a small query or service method here, or better, use mediator.
        // I'll create a simple query to get grading context if needed, but for now I'll just resolve naming.
        // Actually, passing ThesisId is better for URL.
        
        var query = new GTMS.Application.Features.Defense.Jury.Queries.GetDefenseJury.GetDefenseJuryQuery { ThesisId = thesisId };
        var juryVm = await _mediator.Send(query);
        
        model.DefenseSessionId = juryVm.DefenseSessionId;
        ViewBag.ThesisTitle = juryVm.ThesisTitle;
        ViewBag.StudentName = juryVm.StudentName;
        
        return View(model);
    }

    [Authorize(Roles = "Advisor,Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnterScore(GTMS.Application.Features.Defense.Grading.Commands.EnterDefenseScore.EnterDefenseScoreCommand command)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _mediator.Send(command);
                TempData["Success"] = "Defense score finalized successfully.";
                // Access ThesisId somehow to redirect back? 
                // We don't have it in command. We can fetch it or just redirect to Dashboard/Index.
                return RedirectToAction("Index", "Home"); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return View(command);
    }
}
