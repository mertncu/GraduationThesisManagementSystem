using GTMS.Domain.Entities.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Evaluation;

public class DefenseSessionConfiguration : IEntityTypeConfiguration<DefenseSession>
{
    public void Configure(EntityTypeBuilder<DefenseSession> builder)
    {
        builder.HasKey(ds => ds.Id);

        builder.Property(ds => ds.Room)
            .HasMaxLength(100);

        builder.Property(ds => ds.OnlineMeetingLink)
            .HasMaxLength(500);

        builder.HasOne(ds => ds.Thesis)
            .WithMany()
            .HasForeignKey(ds => ds.ThesisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ds => ds.Committee)
            .WithMany()
            .HasForeignKey(ds => ds.CommitteeId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(ds => ds.DefenseStatus)
            .WithMany()
            .HasForeignKey(ds => ds.DefenseStatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
