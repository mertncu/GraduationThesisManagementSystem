using GTMS.Application.Common.Interfaces;
using GTMS.Application.Common.Exceptions;
using GTMS.Domain.Entities.Thesis;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProposals.Commands.ReviewThesisProposal;

public class ReviewThesisProposalCommandHandler : IRequestHandler<ReviewThesisProposalCommand>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ReviewThesisProposalCommandHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(ReviewThesisProposalCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (currentUserId == null) throw new UnauthorizedAccessException();

        var proposal = await _context.ThesisApplications
            .Include(t => t.Student)
            .Include(t => t.Term)
            .FirstOrDefaultAsync(t => t.Id == request.ProposalId, cancellationToken);
        
        if (proposal == null) throw new NotFoundException("Proposal not found");
        
        // Security Check: Only the assigned advisor can review
        if (proposal.AdvisorId != currentUserId)
        {
            throw new UnauthorizedAccessException("Only the assigned advisor can review this proposal.");
        }

        var statusName = request.IsApproved ? "Approved" : "Rejected";
        var statusEntity = await _context.ApplicationStatuses.FirstOrDefaultAsync(s => s.Name == statusName, cancellationToken);
        if (statusEntity == null) throw new InvalidOperationException($"Status '{statusName}' not found in DB.");

        // Update Proposal
        proposal.ApplicationStatusId = statusEntity.Id;
        proposal.DecidedAt = DateTime.UtcNow;
        proposal.DecidedByUserId = currentUserId;
        proposal.AdvisorComment = request.Comment;
        proposal.UpdatedAt = DateTime.UtcNow;

        if (request.IsApproved)
        {
            // Logic to Create Thesis Project
            // 1. Get Initial Thesis Status (e.g. "Ongoing" or "Active")
            var thesisStatus = await _context.ThesisStatuses.FirstOrDefaultAsync(s => s.Name == "Ongoing", cancellationToken);
            if (thesisStatus == null) thesisStatus = await _context.ThesisStatuses.FirstOrDefaultAsync(s => s.Name == "Active", cancellationToken);
             if (thesisStatus == null) throw new InvalidOperationException("Initial Thesis Status not found.");


            // 2. Fetch Student detailed info (Department, Program)
            // Note: Currently ThesisApplication has Navigation to User Student, but we need Student ENTITY for Department/Program IDs.
            var studentEntity = await _context.Students.FirstOrDefaultAsync(s => s.UserId == proposal.StudentId, cancellationToken);
            if (studentEntity == null) throw new InvalidOperationException("Student academic record not found.");

            var project = new ThesisProject
            {
                Id = Guid.NewGuid(),
                StudentId = proposal.StudentId, // This is UserID likely, checking ThesisProject.cs logic
                MainAdvisorId = proposal.AdvisorId,
                Title = proposal.ProposedTitle,
                Abstract = proposal.ProposedAbstract,
                TermId = proposal.TermId,
                DepartmentId = studentEntity.DepartmentId,
                ProgramId = studentEntity.ProgramId,
                ThesisStatusId = thesisStatus.Id,
                ApprovedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.ThesisProjects.Add(project);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
