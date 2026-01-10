using GTMS.Application.Common.Exceptions;
using GTMS.Application.Features.Thesis.ThesisProposals.Commands.CreateThesisProposal;
using GTMS.Application.Features.Thesis.ThesisProposals.Commands.ReviewThesisProposal;
using GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetCreateProposalPage;
using GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetProposalForReview;
using GTMS.Application.Features.Thesis.ThesisProposals.Queries.GetThesisProposals;
using GTMS.Application.Features.Thesis.ThesisProposals.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Authorize]
public class ThesisProposalsController : Controller
{
    private readonly IMediator _mediator;

    public ThesisProposalsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _mediator.Send(new GetThesisProposalsQuery()));
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Create()
    {
        var vm = await _mediator.Send(new GetCreateProposalPageQuery());
        if (!vm.CanCreate)
        {
            TempData["Warning"] = vm.WarningMessage;
            return RedirectToAction(nameof(Index));
        }
        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProposalPageVm model)
    {
        if (!ModelState.IsValid)
        {
             var vm = await _mediator.Send(new GetCreateProposalPageQuery());
             // Copy input back
             vm.Title = model.Title;
             vm.Abstract = model.Abstract;
             vm.AdvisorId = model.AdvisorId;
             vm.TermId = model.TermId;
             return View(vm);
        }

        try 
        {
            await _mediator.Send(new CreateThesisProposalCommand
            {
                Title = model.Title,
                Abstract = model.Abstract,
                AdvisorId = model.AdvisorId,
                TermId = model.TermId
            });
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error creating proposal: " + ex.Message);
            var vm = await _mediator.Send(new GetCreateProposalPageQuery());
             // Copy input back
             vm.Title = model.Title;
             vm.Abstract = model.Abstract;
             vm.AdvisorId = model.AdvisorId;
             vm.TermId = model.TermId;
             return View(vm);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Advisor")]
    public async Task<IActionResult> Review(Guid id)
    {
        try
        {
            return View(await _mediator.Send(new GetProposalForReviewQuery(id)));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }

    [HttpPost]
    [Authorize(Roles = "Advisor")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Review(ReviewProposalVm model)
    {
        try
        {
             await _mediator.Send(new ReviewThesisProposalCommand(
                 model.ProposalId,
                 model.IsApproved,
                 model.Comment
             ));
             return RedirectToAction(nameof(Index));
        }
        catch(Exception ex)
        {
             ModelState.AddModelError("", ex.Message);
             return View(model);
        }
    }
}

