using CO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.EntityConfigurations;

internal sealed class DirectorConfiguration : IEntityTypeConfiguration<Director>
{
    public void Configure(EntityTypeBuilder<Director> builder)
    {
        builder.ToTable("Directors");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClientId)
            .IsRequired();

        builder.Property(x => x.FullName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(x => x.RelationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.IdentificationType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(100);

        builder.Property(x => x.IdentificationNumber)
            .IsRequired()
            .HasMaxLength(100);

        // Unique per client — same person can't appear twice on same client
        builder.HasIndex(x => new { x.ClientId, x.IdentificationNumber })
            .IsUnique();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(30);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.SharePercentage)
            .HasColumnType("decimal(5,2)");

        builder.Property(x => x.DateAdded)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();
    }

}
