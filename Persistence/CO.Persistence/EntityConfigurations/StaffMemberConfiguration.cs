using CO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.EntityConfigurations;

internal sealed class StaffMemberConfiguration : IEntityTypeConfiguration<StaffMember>
{
    public void Configure(EntityTypeBuilder<StaffMember> builder)
    {
        builder.ToTable("StaffMembers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.StaffNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.StaffNumber)
            .IsUnique();

        builder.Property(x => x.Department)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.CreatedAt)
            .IsRequired()
            .HasColumnType("datetimeoffset");

        builder.Property(x => x.RowVersion)
            .IsRowVersion()         
            .IsRequired();


    }
}