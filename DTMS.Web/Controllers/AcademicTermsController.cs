using GTMS.Application.Features.Academic.AcademicTerms.Commands.CreateAcademicTerm;
using GTMS.Application.Features.Academic.AcademicTerms.Commands.DeleteAcademicTerm;
using GTMS.Application.Features.Academic.AcademicTerms.Commands.UpdateAcademicTerm;
using GTMS.Application.Features.Academic.AcademicTerms.Queries.GetAcademicTerms;
using GTMS.Application.Features.Academic.AcademicTerms.Queries.GetAcademicTermById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AcademicTermsController : Controller
{
    private readonly IMediator _mediator;

    public AcademicTermsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var terms = await _mediator.Send(new GetAcademicTermsQuery());
        return View(terms);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAcademicTermCommand command)
    {
        if (!ModelState.IsValid) return View(command);

        await _mediator.Send(command);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var term = await _mediator.Send(new GetAcademicTermByIdQuery(id));
            var model = new UpdateAcademicTermCommand(term.Id, term.Name, term.StartDate, term.EndDate);
            return View(model);
        }
        catch (GTMS.Application.Common.Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateAcademicTermCommand command)
    {
        if (id != command.Id) return BadRequest();
        if (!ModelState.IsValid) return View(command);

        await _mediator.Send(command);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mediator.Send(new DeleteAcademicTermCommand(id));
        return RedirectToAction(nameof(Index));
    }
}
