using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Persistence.Configurations;

public class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable("ContactMessages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.SenderName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(m => m.SenderEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.MessageBody)
            .IsRequired();

        builder.Property(m => m.IpAddress)
            .HasMaxLength(64);

        builder.HasIndex(m => m.SubmittedAtUtc)
            .HasDatabaseName("IX_ContactMessages_SubmittedAtUtc");

        builder.HasIndex(m => m.IsRead)
            .HasDatabaseName("IX_ContactMessages_IsRead");
    }
}
