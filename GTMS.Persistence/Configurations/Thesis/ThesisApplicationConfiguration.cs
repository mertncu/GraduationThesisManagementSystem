using GTMS.Domain.Entities.Thesis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Thesis;

public class ThesisApplicationConfiguration : IEntityTypeConfiguration<ThesisApplication>
{
    public void Configure(EntityTypeBuilder<ThesisApplication> builder)
    {
        builder.HasOne(t => t.Student)
            .WithMany()
            .HasForeignKey(t => t.StudentId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cycle with User

        builder.HasOne(t => t.Advisor)
            .WithMany()
            .HasForeignKey(t => t.AdvisorId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cycle with User

        builder.HasOne(t => t.Term)
            .WithMany()
            .HasForeignKey(t => t.TermId)
            .OnDelete(DeleteBehavior.Restrict);
            
         builder.HasOne(t => t.DecidedByUser)
            .WithMany()
            .HasForeignKey(t => t.DecidedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
