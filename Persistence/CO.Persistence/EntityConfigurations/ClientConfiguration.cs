using CO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CO.Persistence.EntityConfigurations;

internal sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);

        // ── Identity ─────────────────────────────────────────────────────
        builder.Property(x => x.ClientNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.ClientNumber)
            .IsUnique();

        // ── Classification ────────────────────────────────────────────────
        builder.Property(x => x.ClientType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.SegmentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.SubSegmentType)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(100);

        // ── Status & Dates ────────────────────────────────────────────────
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(x => x.OpenedOn)
            .IsRequired()
            .HasColumnType("datetimeoffset");

        // ── Relationship Manager ──────────────────────────────────────────
        builder.Property(x => x.RelationshipManagerId)
            .IsRequired();

        builder.Property(x => x.RowVersion)
            .IsRowVersion();

        builder.HasOne(x => x.RelationshipManager)
            .WithMany()
            .HasForeignKey(x => x.RelationshipManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Owned: CorporateDetails ───────────────────────────────────────
        builder.OwnsOne(x => x.CorporateDetail, cd =>
        {
            cd.Property(x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(300)
                .HasColumnName("CompanyName");

            cd.Property(x => x.LineOfBusiness)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(100)
                .HasColumnName("LineOfBusiness");

            cd.Property(x => x.LineOfBusinessMoreInfo)
                .HasMaxLength(500)
                .HasColumnName("LineOfBusinessMoreInfo");

            cd.Property(x => x.NatureOfBusiness)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("NatureOfBusiness");

            cd.Property(x => x.IdentificationType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(100)
                .HasColumnName("IdentificationType");

            cd.Property(x => x.RegistrationNumber)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("RegistrationNumber");

            cd.HasIndex(x => x.RegistrationNumber)
                .IsUnique();

            cd.Property(x => x.DateOfRegistration)
                .IsRequired()
                .HasColumnType("datetimeoffset")
                .HasColumnName("DateOfRegistration");

            cd.Property(x => x.RegisteredAt)
                .HasMaxLength(200)
                .HasColumnName("RegisteredAt");

            cd.Property(x => x.RegisteredOffice)
                .HasMaxLength(300)
                .HasColumnName("RegisteredOffice");

            cd.Property(x => x.BusinessStartedYear)
                .HasColumnName("BusinessStartedYear");

            cd.Property(x => x.NumberOfEmployees)
                .HasColumnName("NumberOfEmployees");

            cd.Property(x => x.Comments)
                .HasMaxLength(1000)
                .HasColumnName("Comments");

            cd.Property(x => x.Website)
                .HasMaxLength(300)
                .HasColumnName("Website");

            cd.Property(x => x.TINNumber)
                .HasMaxLength(50)
                .HasColumnName("TINNumber");
        });

        // ── Owned: Address ────────────────────────────────────────────────
        builder.OwnsOne(x => x.Address, a =>
        {
            a.Property(x => x.ResidentialAddress)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("ResidentialAddress");

            a.Property(x => x.BusinessAddress)
                .HasMaxLength(500)
                .HasColumnName("BusinessAddress");

            a.Property(x => x.OfficeAddress)
                .HasMaxLength(500)
                .HasColumnName("OfficeAddress");

            a.Property(x => x.MailingAddress)
                .HasMaxLength(500)
                .HasColumnName("MailingAddress");

            a.Property(x => x.HomeCountryAddress)
                .HasMaxLength(500)
                .HasColumnName("HomeCountryAddress");

            a.Property(x => x.AddressLine2)
                .HasMaxLength(500)
                .HasColumnName("AddressLine2");

            a.Property(x => x.Country)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Country");

            a.Property(x => x.Region)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Region");

            a.Property(x => x.Ward)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("Ward");

            a.Property(x => x.District)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("District");

            a.Property(x => x.Street)
                .HasMaxLength(200)
                .HasColumnName("Street");

            a.Property(x => x.ZipCode)
                .HasMaxLength(20)
                .HasColumnName("ZipCode");

            a.Property(x => x.PhoneHome)
                .HasMaxLength(30)
                .HasColumnName("PhoneHome");

            a.Property(x => x.PhoneWork)
                .HasMaxLength(30)
                .HasColumnName("PhoneWork");

            a.Property(x => x.Mobile)
                .HasMaxLength(30)
                .HasColumnName("Mobile");

            a.Property(x => x.FaxNo)
                .HasMaxLength(30)
                .HasColumnName("FaxNo");

            a.Property(x => x.LandMark)
                .HasMaxLength(300)
                .HasColumnName("LandMark");

            a.Property(x => x.EmailId)
                .HasMaxLength(200)
                .HasColumnName("EmailId");
        });

        // ── Owned: CommunicationPrefs ─────────────────────────────────────
        builder.OwnsOne(x => x.CommunicationPreference, cp =>
        {
            cp.Property(x => x.CanSendGreetings)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("CanSendGreetings");

            cp.Property(x => x.CanSendAssociateSpecialOffer)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("CanSendAssociateSpecialOffer");

            cp.Property(x => x.CanSendOurSpecialOffers)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("CanSendOurSpecialOffers");

            cp.Property(x => x.StatementOnline)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("StatementOnline");

            cp.Property(x => x.MobileAlert)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnName("MobileAlert");
        });

        // ── Child: Directors ──────────────────────────────────────────────
        builder.HasMany(x => x.Directors)
            .WithOne()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Ignore domain events — not persisted ──────────────────────────
        builder.Ignore(x => x.DomainEvents);
    }

}
