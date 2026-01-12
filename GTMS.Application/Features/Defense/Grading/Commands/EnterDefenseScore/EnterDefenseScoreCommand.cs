using FluentValidation;
using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Defense;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Defense.Grading.Commands.EnterDefenseScore;

public class EnterDefenseScoreCommand : IRequest<bool>
{
    public Guid DefenseSessionId { get; set; }
    public double QualityScore { get; set; }
    public double PresentationScore { get; set; }
    public double QAScore { get; set; }
    public string? Comment { get; set; }
}

public class EnterDefenseScoreCommandValidator : AbstractValidator<EnterDefenseScoreCommand>
{
    public EnterDefenseScoreCommandValidator()
    {
        RuleFor(v => v.DefenseSessionId).NotEmpty();
        RuleFor(v => v.QualityScore).InclusiveBetween(0, 100);
        RuleFor(v => v.PresentationScore).InclusiveBetween(0, 100);
        RuleFor(v => v.QAScore).InclusiveBetween(0, 100);
    }
}

public class EnterDefenseScoreCommandHandler : IRequestHandler<EnterDefenseScoreCommand, bool>
{
    private readonly IGtmsDbContext _context;

    public EnterDefenseScoreCommandHandler(IGtmsDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EnterDefenseScoreCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.DefenseSessions
            .Include(d => d.ThesisProject)
            .FirstOrDefaultAsync(d => d.Id == request.DefenseSessionId, cancellationToken);

        if (session == null)
            throw new ArgumentException("Defense Session not found.");

        // Calculate Total (Simple Average for now)
        double total = (request.QualityScore + request.PresentationScore + request.QAScore) / 3.0;
        string result = total >= 70 ? "Successful" : "Unsuccessful";

        session.QualityScore = request.QualityScore;
        session.PresentationScore = request.PresentationScore;
        session.QAScore = request.QAScore;
        session.TotalScore = Math.Round(total, 2);
        session.Result = result;
        session.Comment = request.Comment;
        session.UpdatedAt = DateTime.UtcNow;

        // Update Project Status
        var newStatusName = result == "Successful" ? "Completed" : "Failed"; // Or "RevisionRequired"
        var newStatus = await _context.ThesisStatuses.FirstOrDefaultAsync(s => s.Name == newStatusName, cancellationToken);
        
        if (newStatus != null)
        {
            session.ThesisProject.ThesisStatusId = newStatus.Id;
            session.ThesisProject.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
