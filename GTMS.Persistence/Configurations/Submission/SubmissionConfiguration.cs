using GTMS.Domain.Entities.Submission;
using GTMS.Domain.Entities.Identity; // For User if needed
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Submission;

public class SubmissionConfiguration : IEntityTypeConfiguration<GTMS.Domain.Entities.Submission.Submission>
{
    public void Configure(EntityTypeBuilder<GTMS.Domain.Entities.Submission.Submission> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.HasOne(s => s.Thesis)
            .WithMany()
            .HasForeignKey(s => s.ThesisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Milestone)
            .WithMany(m => m.Submissions)
            .HasForeignKey(s => s.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Student)
            .WithMany()
            .HasForeignKey(s => s.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // ReviewedByUserId is nullable and not an entity Ref property in class, just ID. 
        // If we want FK constraint, we can add HasOne<User>().WithMany().HasForeignKey...
        // Assuming we want strict integrity:
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(s => s.ReviewedByUserId)
            .OnDelete(DeleteBehavior.SetNull); 
    }
}
