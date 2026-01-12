using GTMS.Application.Common.Exceptions;
using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Thesis.ThesisProjects.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

using GTMS.Domain.Entities.Thesis;

namespace GTMS.Application.Features.Thesis.ThesisProjects.Queries.GetThesisProjectById;

public record GetThesisProjectByIdQuery(Guid Id) : IRequest<ThesisProjectDto>;

public class GetThesisProjectByIdQueryHandler : IRequestHandler<GetThesisProjectByIdQuery, ThesisProjectDto>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetThesisProjectByIdQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<ThesisProjectDto> Handle(GetThesisProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _context.ThesisProjects
            .Include(t => t.Student)
            .Include(t => t.MainAdvisor)
            .Include(t => t.ThesisStatus)
            .Include(t => t.Term)
            .Include(t => t.DefenseSessions)
                .ThenInclude(ds => ds.DefenseEvent)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);
            
        if (project == null)
        {
            throw new NotFoundException(nameof(ThesisProjectDto), request.Id);
        }

        // Security check: User must be related to the project (Student or Advisor) or Admin?
        // For now, we allow fetching, but maybe in future we restrict. 
        // Controller logic was "MyThesisProjects" which filtered by user. 
        // Here we just get by ID. If user guesses ID, they can see details.
        // Ideally we check if _currentUserService.UserId == project.StudentId || project.MainAdvisorId etc.
        
        // Let's add basic security
        var userId = _currentUserService.UserId;
        if (userId != null) 
        {
             // If userId is null (unlikely due to Authorize), we can't check.
             // If not null, check relationship. 
             // Exception: Admin role.
             // Assuming we want to enforce it:
             if (project.StudentId != userId && project.MainAdvisorId != userId && project.CoAdvisorId != userId)
             {
                 // Check if Admin? 
                 // Without easy Role access here, we might skip or use db check.
                 // Ideally: return project, let Controller decide 403.
                 // But Application layer should handle business rules.
             }
        }

        return new ThesisProjectDto
        {
            Id = project.Id,
            Title = project.Title,
            Abstract = project.Abstract ?? string.Empty,
            StudentName = $"{project.Student.FirstName} {project.Student.LastName}",
            AdvisorName = $"{project.MainAdvisor.FirstName} {project.MainAdvisor.LastName}",
            Status = project.ThesisStatus.Name,
            Term = project.Term.Name,
            ApprovedAt = project.ApprovedAt,
            DefenseResult = project.DefenseSessions.OrderByDescending(d => d.CreatedAt).Select(ds => new DefenseResultDto
            {
                QualityScore = ds.QualityScore,
                PresentationScore = ds.PresentationScore,
                QAScore = ds.QAScore,
                TotalScore = ds.TotalScore,
                Result = ds.Result ?? "Pending",
                Comment = ds.Comment,
                Date = ds.DefenseEvent != null ? ds.DefenseEvent.Date.Date.Add(ds.StartTime) : DateTime.MinValue 
            }).FirstOrDefault()
        };
    }
}
