using GTMS.Application.Features.Thesis.ThesisMilestones.Queries.GetThesisMilestones;
using GTMS.Application.Features.Thesis.ThesisProjects.Queries.GetMyThesisProjects;
using GTMS.Application.Features.Thesis.ThesisProjects.Queries.GetThesisProjectById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Authorize]
public class ThesisProjectsController : Controller
{
    private readonly IMediator _mediator;

    public ThesisProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var projects = await _mediator.Send(new GetMyThesisProjectsQuery());
        return View(projects);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var project = await _mediator.Send(new GetThesisProjectByIdQuery(id));
            
            var milestones = await _mediator.Send(new GetThesisMilestonesQuery(id));

            var model = new ProjectDetailsViewModel
            {
                Project = project,
                Milestones = milestones
            };

            return View(model);
        }
        catch (GTMS.Application.Common.Exceptions.NotFoundException)
        {
            return NotFound();
        }
    }
}

public class ProjectDetailsViewModel
{
    public GTMS.Application.Features.Thesis.ThesisProjects.Dtos.ThesisProjectDto Project { get; set; }
    public List<GTMS.Application.Features.Thesis.ThesisMilestones.Dtos.ThesisMilestoneDto> Milestones { get; set; }
}
