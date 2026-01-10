using GTMS.Application.Features.Thesis.MonthlyReports.Commands.CreateMonthlyReport;
using GTMS.Application.Features.Thesis.MonthlyReports.Queries.GetMonthlyReportsByThesisId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Authorize]
public class MonthlyReportsController : Controller
{
    private readonly IMediator _mediator;

    public MonthlyReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: MonthlyReports?thesisId=...
    public async Task<IActionResult> Index(Guid thesisId)
    {
        if (thesisId == Guid.Empty) return BadRequest("ThesisId is required");
        
        // Pass ThesisId via ViewBag or ViewModel wrapper. 
        // Queries return List<Dto>. Let's pack it in a simple anonymous object or ViewBag for now, or better:
        // Use a VM. But to save time and stick to requirement:
        ViewBag.ThesisId = thesisId;
        
        // Check if user has access ideally.
        
        var reports = await _mediator.Send(new GetMonthlyReportsByThesisIdQuery(thesisId));
        return View(reports);
    }

    [HttpGet]
    public IActionResult Create(Guid thesisId)
    {
        return View(new CreateMonthlyReportCommand { ThesisId = thesisId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMonthlyReportCommand command)
    {
        await _mediator.Send(command);
        return RedirectToAction(nameof(Index), new { thesisId = command.ThesisId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Advisor")]
    public async Task<IActionResult> Review(Guid thesisId, Guid reportId, string comment, bool isApproved)
    {
        await _mediator.Send(new GTMS.Application.Features.Thesis.MonthlyReports.Commands.ReviewMonthlyReport.ReviewMonthlyReportCommand
        {
            ReportId = reportId,
            Comment = comment,
            IsApproved = isApproved
        });
        
        return RedirectToAction(nameof(Index), new { thesisId = thesisId });
    }
}
