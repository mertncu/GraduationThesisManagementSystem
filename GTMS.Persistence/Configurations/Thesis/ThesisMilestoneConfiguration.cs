using GTMS.Domain.Entities.Thesis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Thesis;

public class ThesisMilestoneConfiguration : IEntityTypeConfiguration<ThesisMilestone>
{
    public void Configure(EntityTypeBuilder<ThesisMilestone> builder)
    {
        builder.HasKey(tm => tm.Id);

        builder.Property(tm => tm.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(tm => tm.Description)
            .HasMaxLength(500);

        builder.HasOne(tm => tm.Thesis)
            .WithMany(t => t.ThesisMilestones)
            .HasForeignKey(tm => tm.ThesisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tm => tm.MilestoneType)
            .WithMany()
            .HasForeignKey(tm => tm.MilestoneTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
