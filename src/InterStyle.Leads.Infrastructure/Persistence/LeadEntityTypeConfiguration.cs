using InterStyle.Leads.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Infrastructure.Persistence;

public sealed class LeadEntityTypeConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("leads");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new LeadId(value))
            .ValueGeneratedNever();

        builder.Property(x => x.CustomerName)
            .HasConversion(name => name.Value, value => CustomerName.Rehydrate(value))
            .HasColumnName("customer_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasConversion(phone => phone.Value, value => PhoneNumber.Rehydrate(value))
            .HasColumnName("phone_number")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion(status => status.Id, id => InterStyle.Shared.Enumeration.FromId<LeadStatus>(id))
            .HasColumnName("status_id")
            .IsRequired();

        builder.Property(x => x.ServiceType)
            .HasConversion(type => type.Id, id => InterStyle.Shared.Enumeration.FromId<LeadServiceType>(id))
            .HasColumnName("service_type_id")
            .IsRequired();

        builder.Property(x => x.Source)
            .HasConversion(source => source.Id, id => InterStyle.Shared.Enumeration.FromId<LeadSource>(id))
            .HasColumnName("source_id")
            .IsRequired();

        builder.OwnsOne(x => x.RequestDetails, owned =>
        {
            owned.Property(x => x.Address).HasColumnName("address").HasMaxLength(400);
            owned.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(2000);
            owned.Property(x => x.RequiresInstallation).HasColumnName("requires_installation");
        });

        builder.Property(x => x.CreatedAtUtc).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasColumnName("updated_at_utc").IsRequired();
    }
}