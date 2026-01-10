using GTMS.Application.Common.Interfaces;
using GTMS.Application.Features.Thesis.ThesisProjects.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Application.Features.Thesis.ThesisProjects.Queries.GetMyThesisProjects;

public class GetMyThesisProjectsQueryHandler : IRequestHandler<GetMyThesisProjectsQuery, List<ThesisProjectDto>>
{
    private readonly IGtmsDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyThesisProjectsQueryHandler(IGtmsDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<List<ThesisProjectDto>> Handle(GetMyThesisProjectsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null) throw new UnauthorizedAccessException();

        var query = _context.ThesisProjects
            .Include(t => t.Student)
            .Include(t => t.MainAdvisor)
            .Include(t => t.ThesisStatus)
            .Include(t => t.Term)
            .AsQueryable();

        // How to determine role? 
        // We can check if user ID matches StudentId or AdvisorId
        // Assuming user can only be one role contextually or we return all matches.
        
        query = query.Where(t => t.StudentId == userId || t.MainAdvisorId == userId || t.CoAdvisorId == userId);

        var projects = await query.Select(t => new ThesisProjectDto
        {
            Id = t.Id,
            Title = t.Title,
            Abstract = t.Abstract,
            StudentName = $"{t.Student.FirstName} {t.Student.LastName}",
            AdvisorName = $"{t.MainAdvisor.FirstName} {t.MainAdvisor.LastName}",
            Status = t.ThesisStatus.Name,
            Term = t.Term.Name,
            ApprovedAt = t.ApprovedAt
        }).ToListAsync(cancellationToken);

        return projects;
    }
}
