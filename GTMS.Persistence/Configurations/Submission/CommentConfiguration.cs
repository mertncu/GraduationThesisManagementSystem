using GTMS.Domain.Entities.Submission;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Submission;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CommentText)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne(c => c.Submission)
            .WithMany() // Assuming Submission has Collection<Comment> Comments? Yes.
            .HasForeignKey(c => c.SubmissionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
