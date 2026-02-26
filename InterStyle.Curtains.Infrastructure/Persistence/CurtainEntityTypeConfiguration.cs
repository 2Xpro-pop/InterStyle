using InterStyle.Curtains.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterStyle.Curtains.Infrastructure.Persistence;

public sealed class CurtainEntityTypeConfiguration : IEntityTypeConfiguration<Curtain>
{
    public void Configure(EntityTypeBuilder<Curtain> builder)
    {
        builder.ToTable("curtains");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new CurtainId(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasConversion(name => name.Value, value => CurtainName.Rehydrate(value))
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(desc => desc.Value, value => Description.Rehydrate(value))
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.PictureUrl)
            .HasConversion(url => url.Value, value => PictureUrl.Rehydrate(value))
            .HasColumnName("picture_url")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.PreviewUrl)
            .HasConversion(url => url.Value, value => PictureUrl.Rehydrate(value))
            .HasColumnName("preview_url")
            .HasMaxLength(2000)
            .IsRequired();
    }
}