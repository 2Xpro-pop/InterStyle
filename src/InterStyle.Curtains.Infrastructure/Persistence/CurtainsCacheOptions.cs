namespace InterStyle.Curtains.Infrastructure.Persistence;

/// <summary>
/// Configuration options for curtain query caching.
/// Bound from the "Caching" configuration section.
/// </summary>
public sealed class CurtainsCacheOptions
{
    public const string SectionName = "Caching";

    /// <summary>
    /// Cache duration in minutes. Default: 5.
    /// </summary>
    public int DurationMinutes { get; set; } = 5;
}
