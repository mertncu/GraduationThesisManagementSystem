using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Thesis.ThesisMilestones.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisMilestones.Queries.GetThesisMilestones;

public class GetThesisMilestonesQueryHandler : IRequestHandler<GetThesisMilestonesQuery, List<ThesisMilestoneDto>>
{
    private readonly IGtmsDbContext _context;

    public GetThesisMilestonesQueryHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<List<ThesisMilestoneDto>> Handle(GetThesisMilestonesQuery request, CancellationToken cancellationToken)
    {
        return await _context.ThesisMilestones
            .Include(m => m.Submissions)
                .ThenInclude(s => s.SubmissionStatus)
            .Include(m => m.Submissions)
                .ThenInclude(s => s.Student)
            .Where(m => m.ThesisId == request.ThesisId)
            .OrderBy(m => m.DueDate)
            .Select(m => new ThesisMilestoneDto
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description ?? string.Empty,
                DueDate = m.DueDate,
                IsLocked = m.IsLocked,
                Submissions = m.Submissions.Select(s => new GTMS.Application.Features.Submission.Submissions.Dtos.SubmissionDto
                {
                    Id = s.Id,
                    MilestoneId = s.MilestoneId,
                    StudentName = $"{s.Student.FirstName} {s.Student.LastName}",
                    Status = s.SubmissionStatus.Name,
                    FilePath = s.FilePath,
                    OriginalFileName = s.OriginalFileName,
                    Notes = s.Notes,
                    Feedback = s.Feedback,
                    SubmittedAt = s.SubmittedAt
                }).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
