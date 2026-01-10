using GTMS.Domain.Entities.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Evaluation;

public class FinalGradeConfiguration : IEntityTypeConfiguration<FinalGrade>
{
    public void Configure(EntityTypeBuilder<FinalGrade> builder)
    {
        builder.HasKey(fg => fg.Id);

        builder.Property(fg => fg.NumericGrade)
            .HasPrecision(5, 2);

        builder.Property(fg => fg.LetterGrade)
            .IsRequired()
            .HasMaxLength(5);

        builder.HasOne(fg => fg.Thesis)
            .WithMany()
            .HasForeignKey(fg => fg.ThesisId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(fg => fg.Rubric)
            .WithMany()
            .HasForeignKey(fg => fg.RubricId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(fg => fg.ApprovedByUser)
            .WithMany()
            .HasForeignKey(fg => fg.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
