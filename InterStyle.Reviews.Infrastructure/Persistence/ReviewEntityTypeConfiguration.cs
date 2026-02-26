using InterStyle.Reviews.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// EF Core entity type configuration for Review aggregate.
/// </summary>
public sealed class ReviewEntityTypeConfiguration : IEntityTypeConfiguration<Review>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("reviews");

        builder.HasKey(x => x.Id);

        builder.Ignore(x => x.DomainEvents);

        builder.Property(x => x.Id)
            .HasConversion(id => id.Value, value => new ReviewId(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.CustomerName)
            .HasConversion(name => name.Value, value => CustomerName.Rehydrate(value))
            .HasColumnName("customer_name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Rating)
            .HasConversion(rating => rating.Value, value => Rating.Rehydrate(value))
            .HasColumnName("rating")
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasConversion(comment => comment.Value, value => ReviewComment.Rehydrate(value))
            .HasColumnName("comment")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(x => x.IsApproved)
            .HasColumnName("is_approved")
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at_utc")
            .IsRequired();

        builder.Property(x => x.ApprovedAtUtc)
            .HasColumnName("approved_at_utc");

        // Index for querying approved reviews (most common query)
        builder.HasIndex(x => new { x.IsApproved, x.CreatedAtUtc })
            .HasDatabaseName("ix_reviews_is_approved_created_at_utc");
    }
}
