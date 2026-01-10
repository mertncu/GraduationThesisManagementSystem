using GTMS.Domain.Entities.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Evaluation;

public class CommitteeConfiguration : IEntityTypeConfiguration<Committee>
{
    public void Configure(EntityTypeBuilder<Committee> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Notes)
            .HasMaxLength(500);

        builder.HasOne(c => c.Thesis)
            .WithMany()
            .HasForeignKey(c => c.ThesisId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
