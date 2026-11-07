using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Portfolio.Domain.Entities;
using System.Text.Json;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.LiveDemoUrl)
            .HasMaxLength(500);

        builder.Property(p => p.GithubUrl)
            .HasMaxLength(500);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        // Storing primitive collections as JSON in SQL Server 2019+
        var jsonConverter = new ValueConverter<List<string>, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());

        var listComparer = new ValueComparer<List<string>>(
            (a, b) => (a == null && b == null) || (a != null && b != null && a.SequenceEqual(b)),
            v => v == null ? 0 : v.Aggregate(0, (hash, item) => hash ^ (item == null ? 0 : item.GetHashCode())),
            v => v == null ? new List<string>() : v.ToList());

        builder.Property(p => p.TechnicalHighlights)
            .HasConversion(jsonConverter, listComparer);

        builder.Property(p => p.TechStack)
            .HasConversion(jsonConverter, listComparer);

        builder.HasIndex(p => new { p.IsFeatured, p.DisplayOrder })
            .HasDatabaseName("IX_Projects_IsFeatured_DisplayOrder");

        builder.HasIndex(p => p.Category)
            .HasDatabaseName("IX_Projects_Category");
    }
}
