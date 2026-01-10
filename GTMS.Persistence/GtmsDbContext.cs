using GTMS.Application.Common.Interfaces;
using GTMS.Domain.Entities.Academic;
using GTMS.Domain.Entities.Evaluation;
using GTMS.Domain.Entities.Identity;
using GTMS.Domain.Entities.Notification;
using GTMS.Domain.Entities.Submission;
using GTMS.Domain.Entities.System;
using GTMS.Domain.Entities.Thesis;
using Microsoft.EntityFrameworkCore;

namespace GTMS.Persistence;

public class GtmsDbContext : DbContext, IGtmsDbContext
{
    public GtmsDbContext(DbContextOptions<GtmsDbContext> options) : base(options)
    {
    }

    // Academic
    public DbSet<AcademicTerm> AcademicTerms { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Advisor> Advisors { get; set; }
    public DbSet<GTMS.Domain.Entities.Academic.Program> Programs { get; set; }

    // Evaluation
    public DbSet<Committee> Committees { get; set; }
    public DbSet<CommitteeMember> CommitteeMembers { get; set; }
    public DbSet<CommitteeRole> CommitteeRoles { get; set; }
    public DbSet<DefenseSession> DefenseSessions { get; set; }
    public DbSet<DefenseStatus> DefenseStatuses { get; set; }
    public DbSet<Evaluation> Evaluations { get; set; }
    public DbSet<EvaluationRubric> EvaluationRubrics { get; set; }
    public DbSet<FinalGrade> FinalGrades { get; set; }
    public DbSet<RubricCriteria> RubricCriterias { get; set; }

    // Identity
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    // Notification
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }

    // Submission
    public DbSet<Attachment> Attachments { get; set; }
    public DbSet<Submission> Submissions { get; set; }
    public DbSet<AttachmentType> AttachmentTypes { get; set; }
    public DbSet<SubmissionStatus> SubmissionStatuses { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<GTMS.Domain.Entities.Thesis.MonthlyReport> MonthlyReports { get; set; }

    // System
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    // Thesis
    public DbSet<ApplicationStatus> ApplicationStatuses { get; set; }
    public DbSet<ThesisMilestone> ThesisMilestones { get; set; }
    public DbSet<MilestoneType> MilestoneTypes { get; set; }
    public DbSet<ThesisProject> ThesisProjects { get; set; }
    public DbSet<ThesisApplication> ThesisApplications { get; set; }
    public DbSet<ThesisStatus> ThesisStatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Scan for external configurations regardless of where they are (if we add them later)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GtmsDbContext).Assembly);
    }
}
