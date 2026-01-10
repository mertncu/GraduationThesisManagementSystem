using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Submission;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Submission.Submissions.Commands.CreateSubmission;

public class CreateSubmissionHandler : IRequestHandler<CreateSubmissionCommand>
{
    private readonly IGtmsDbContext _context;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUserService;

    public CreateSubmissionHandler(IGtmsDbContext context, IFileStorageService fileStorage, ICurrentUserService currentUserService)
    {
        _context = context;
        _fileStorage = fileStorage;
        _currentUserService = currentUserService;
    }

    public async Task Handle(CreateSubmissionCommand request, CancellationToken cancellationToken)
    {
        string? relativePath = null;
        if (request.FileContent != null && !string.IsNullOrEmpty(request.FileName))
        {
             string folder = $"uploads/{request.ThesisId}";
             relativePath = await _fileStorage.SaveFileAsync(request.FileContent, request.FileName, folder);
        }

        var studentId = _currentUserService.UserId ?? throw new UnauthorizedAccessException();
        
        // Fetch 'Submitted' status.
        var status = await _context.SubmissionStatuses.FirstOrDefaultAsync(s => s.Name == "Submitted", cancellationToken);
        if (status == null) throw new InvalidOperationException("Submission Status 'Submitted' not found in database.");

        var submission = new Domain.Entities.Submission.Submission
        {
             ThesisId = request.ThesisId,
             MilestoneId = request.MilestoneId,
             StudentId = studentId,
             Notes = request.Notes ?? string.Empty,
             FilePath = relativePath,
             OriginalFileName = request.FileName,
             SubmittedAt = DateTime.UtcNow,
             SubmissionStatusId = status.Id
        };

        _context.Submissions.Add(submission);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
