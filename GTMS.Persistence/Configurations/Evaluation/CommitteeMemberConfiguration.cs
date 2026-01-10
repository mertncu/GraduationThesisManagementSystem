using GTMS.Domain.Entities.Evaluation;
using GTMS.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Evaluation;

public class CommitteeMemberConfiguration : IEntityTypeConfiguration<CommitteeMember>
{
    public void Configure(EntityTypeBuilder<CommitteeMember> builder)
    {
        builder.HasKey(cm => cm.Id);

        builder.HasOne(cm => cm.Committee)
            .WithMany() // Add ICollection<CommitteeMember> to Committee? Usually yes.
            .HasForeignKey(cm => cm.CommitteeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cm => cm.Advisor)
            .WithMany()
            .HasForeignKey(cm => cm.AdvisorId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(cm => cm.CommitteeRole)
            .WithMany()
            .HasForeignKey(cm => cm.CommitteeRoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
