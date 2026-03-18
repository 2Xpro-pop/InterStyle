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

        builder.OwnsMany(x => x.Translations, tb =>
        {
            tb.ToTable("curtain_translations");

            tb.WithOwner().HasForeignKey("curtain_id");

            tb.Property<Guid>("id");
            tb.HasKey("id");

            tb.Property(x => x.Locale)
                .HasConversion(l => l.Value, v => Locale.Rehydrate(v))
                .HasColumnName("locale")
                .HasMaxLength(16)
                .IsRequired();

            tb.Property(x => x.Name)
                .HasConversion(n => n.Value, v => CurtainName.Rehydrate(v))
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();

            tb.Property(x => x.Description)
                .HasConversion(d => d.Value, v => Description.Rehydrate(v))
                .HasColumnName("description")
                .HasMaxLength(500)
                .IsRequired();

            tb.HasIndex("curtain_id", "Locale")
                .IsUnique();
        });

        builder.Navigation(x => x.Translations)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}