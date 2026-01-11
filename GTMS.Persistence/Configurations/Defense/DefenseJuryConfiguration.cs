using GTMS.Domain.Entities.Defense;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Defense;

public class DefenseJuryConfiguration : IEntityTypeConfiguration<DefenseJury>
{
    public void Configure(EntityTypeBuilder<DefenseJury> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.DefenseSession)
            .WithMany(x => x.JuryMembers)
            .HasForeignKey(x => x.DefenseSessionId);

        builder.HasOne(x => x.Advisor)
            .WithMany()
            .HasForeignKey(x => x.AdvisorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
