using GTMS.Application.Features.Submission.Submissions.Commands.CreateSubmission;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DTMS.Web.Controllers;

[Authorize]
public class SubmissionsController : Controller
{
    private readonly IMediator _mediator;

    public SubmissionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSubmissionViewModel model)
    {
        if (model.File == null || model.File.Length == 0)
        {
             TempData["Error"] = "Please select a file.";
             return RedirectToAction("Details", "ThesisProjects", new { id = model.ThesisId });
        }

        using (var stream = model.File.OpenReadStream())
        {
            var command = new CreateSubmissionCommand
            {
                ThesisId = model.ThesisId,
                MilestoneId = model.MilestoneId,
                Notes = model.Notes,
                FileContent = stream,
                FileName = model.File.FileName
            };

            await _mediator.Send(command);
        }

        return RedirectToAction("Details", "ThesisProjects", new { id = model.ThesisId });
    }

    [HttpPost]
    [Authorize(Roles = "Advisor")]
    public async Task<IActionResult> Review(ReviewSubmissionViewModel model)
    {
        var command = new GTMS.Application.Features.Submission.Submissions.Commands.ReviewSubmission.ReviewSubmissionCommand
        {
            SubmissionId = model.SubmissionId,
            Status = model.Status,
            Feedback = model.Feedback
        };

        await _mediator.Send(command);

        // We need ThesisId for redirect. Ideally passed in view model or fetched. 
        // For simplicity, passing ThesisId from form.
        return RedirectToAction("Details", "ThesisProjects", new { id = model.ThesisId });
    }
}

public class CreateSubmissionViewModel
{
    public Guid ThesisId { get; set; }
    public Guid MilestoneId { get; set; }
    public string? Notes { get; set; }
    public IFormFile? File { get; set; }
}

public class ReviewSubmissionViewModel
{
    public Guid ThesisId { get; set; } // For redirect
    public Guid SubmissionId { get; set; }
    public string Status { get; set; } = null!;
    public string? Feedback { get; set; }
}
