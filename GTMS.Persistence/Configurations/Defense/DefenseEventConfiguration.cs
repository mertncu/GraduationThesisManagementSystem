using GTMS.Domain.Entities.Defense;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Defense;

public class DefenseEventConfiguration : IEntityTypeConfiguration<DefenseEvent>
{
    public void Configure(EntityTypeBuilder<DefenseEvent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Term)
            .WithMany()
            .HasForeignKey(x => x.TermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Sessions)
            .WithOne(x => x.DefenseEvent)
            .HasForeignKey(x => x.DefenseEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
