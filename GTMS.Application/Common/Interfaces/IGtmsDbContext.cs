using Microsoft.EntityFrameworkCore;
using GTMS.Domain.Entities.Academic;
using GTMS.Domain.Entities.Evaluation;
using GTMS.Domain.Entities.Identity;
using GTMS.Domain.Entities.Submission;
using GTMS.Domain.Entities.Thesis;
using GTMS.Domain.Entities.Notification;
using GTMS.Domain.Entities.System;

namespace GTMS.Application.Common.Interfaces;

public interface IGtmsDbContext
{
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    
    DbSet<AcademicTerm> AcademicTerms { get; }
    DbSet<Department> Departments { get; }
    DbSet<GTMS.Domain.Entities.Academic.Program> Programs { get; }
    DbSet<Student> Students { get; }
    DbSet<Advisor> Advisors { get; }
    
    DbSet<ThesisProject> ThesisProjects { get; }
    DbSet<ThesisStatus> ThesisStatuses { get; }
    DbSet<ThesisApplication> ThesisApplications { get; }
    DbSet<ApplicationStatus> ApplicationStatuses { get; }
    DbSet<ThesisMilestone> ThesisMilestones { get; }
    DbSet<MilestoneType> MilestoneTypes { get; }
    
    DbSet<Submission> Submissions { get; }
    DbSet<SubmissionStatus> SubmissionStatuses { get; }
    DbSet<Comment> Comments { get; }
    DbSet<GTMS.Domain.Entities.Thesis.MonthlyReport> MonthlyReports { get; }
    
    DbSet<Committee> Committees { get; }
    DbSet<CommitteeMember> CommitteeMembers { get; }
    DbSet<DefenseSession> DefenseSessions { get; }
    DbSet<Evaluation> Evaluations { get; }
    DbSet<FinalGrade> FinalGrades { get; }
    
    DbSet<Notification> Notifications { get; }
    DbSet<ActivityLog> ActivityLogs { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
