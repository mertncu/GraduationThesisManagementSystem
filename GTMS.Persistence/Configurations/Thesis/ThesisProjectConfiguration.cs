using GTMS.Domain.Entities.Thesis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Thesis;

public class ThesisProjectConfiguration : IEntityTypeConfiguration<ThesisProject>
{
    public void Configure(EntityTypeBuilder<ThesisProject> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(t => t.Abstract)
            .HasMaxLength(2000);

        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        // Relationships with User
        builder.HasOne(t => t.Student)
            .WithMany()
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.MainAdvisor)
            .WithMany()
            .HasForeignKey(t => t.MainAdvisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.CoAdvisor)
            .WithMany()
            .HasForeignKey(t => t.CoAdvisorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(t => t.Department)
            .WithMany()
            .HasForeignKey(t => t.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Program)
            .WithMany()
            .HasForeignKey(t => t.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Term)
            .WithMany()
            .HasForeignKey(t => t.TermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.DefenseSessions)
            .WithOne(d => d.ThesisProject)
            .HasForeignKey(d => d.ThesisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
