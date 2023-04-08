using Bottled.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bottled.Api.Infrastructure.Data.Mappings;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder
            .HasKey(b => b.Id)
            .HasName("id");

        builder
            .Property(b => b.Author)
            .HasColumnName("author")
            .HasMaxLength(50)
            .IsRequired();

        builder
            .Property(b => b.Content)
            .HasColumnName("content")
            .HasMaxLength(255)
            .IsRequired();

        builder.ToTable("message");
    }
}
