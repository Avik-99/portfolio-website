using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("Skills");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Category)
            .IsRequired();

        builder.Property(s => s.IconClass)
            .HasMaxLength(100);

        builder.HasIndex(s => new { s.Category, s.DisplayOrder })
            .HasDatabaseName("IX_Skills_Category_DisplayOrder");
    }
}
