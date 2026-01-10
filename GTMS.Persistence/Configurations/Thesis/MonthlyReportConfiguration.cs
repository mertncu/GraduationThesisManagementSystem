using GTMS.Domain.Entities.Thesis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Thesis;

public class MonthlyReportConfiguration : IEntityTypeConfiguration<MonthlyReport>
{
    public void Configure(EntityTypeBuilder<MonthlyReport> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(4000); // Reasonable limit for a report

        builder.Property(x => x.AdvisorComment)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.Thesis)
            .WithMany() // Assuming we don't need a navigation collection on ThesisProject for now, or we can add it later
            .HasForeignKey(x => x.ThesisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Student)
            .WithMany()
            .HasForeignKey(x => x.StudentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
