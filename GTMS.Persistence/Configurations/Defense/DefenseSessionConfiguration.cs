using GTMS.Domain.Entities.Defense;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Defense;

public class DefenseSessionConfiguration : IEntityTypeConfiguration<DefenseSession>
{
    public void Configure(EntityTypeBuilder<DefenseSession> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.DefenseEvent)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.DefenseEventId);

        builder.HasOne(x => x.ThesisProject)
            .WithMany() 
            .HasForeignKey(x => x.ThesisId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.JuryMembers)
            .WithOne(x => x.DefenseSession)
            .HasForeignKey(x => x.DefenseSessionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
