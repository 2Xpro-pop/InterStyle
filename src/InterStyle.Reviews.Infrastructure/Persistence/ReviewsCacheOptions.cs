namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// Configuration options for review query caching.
/// Bound from the "Caching" configuration section.
/// </summary>
public sealed class ReviewsCacheOptions
{
    public const string SectionName = "Caching";

    /// <summary>
    /// Cache duration in minutes. Default: 2.
    /// </summary>
    public int DurationMinutes { get; set; } = 2;

    /// <summary>
    /// Maximum number of approved reviews to cache in a single snapshot. Default: 100.
    /// Requests beyond this range fall through to the database.
    /// </summary>
    public int MaxCachedItems { get; set; } = 100;
}
