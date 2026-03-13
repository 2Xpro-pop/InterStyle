namespace InterStyle.Curtains.Application;

/// <summary>
/// Invalidates cached curtain query results.
/// </summary>
public interface ICurtainCacheInvalidator
{
    Task InvalidateAllCurtainsAsync(CancellationToken cancellationToken = default);
}
