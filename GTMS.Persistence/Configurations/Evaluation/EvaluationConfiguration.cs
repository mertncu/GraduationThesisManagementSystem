using GTMS.Domain.Entities.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Evaluation;

public class EvaluationConfiguration : IEntityTypeConfiguration<GTMS.Domain.Entities.Evaluation.Evaluation>
{
    public void Configure(EntityTypeBuilder<GTMS.Domain.Entities.Evaluation.Evaluation> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Score)
            .HasPrecision(5, 2);

        builder.Property(e => e.Comment)
            .HasMaxLength(1000);

        builder.HasOne(e => e.DefenseSession)
            .WithMany() // Assuming no collection on DefenseSession for specific evaluations yet
            .HasForeignKey(e => e.DefenseSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.CommitteeMember)
            .WithMany()
            .HasForeignKey(e => e.CommitteeMemberId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting member if they have evals

        builder.HasOne(e => e.RubricCriteria)
            .WithMany()
            .HasForeignKey(e => e.RubricCriteriaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
