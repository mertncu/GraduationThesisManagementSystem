using GTMS.Domain.Entities.Academic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GTMS.Persistence.Configurations.Academic;

public class ProgramConfiguration : IEntityTypeConfiguration<GTMS.Domain.Entities.Academic.Program>
{
    public void Configure(EntityTypeBuilder<GTMS.Domain.Entities.Academic.Program> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasOne(p => p.Department)
            .WithMany() // Department has Programs?
            .HasForeignKey(p => p.DepartmentId) // Assuming DepartmentId exists on Program
            .OnDelete(DeleteBehavior.Cascade); 
    }
}
