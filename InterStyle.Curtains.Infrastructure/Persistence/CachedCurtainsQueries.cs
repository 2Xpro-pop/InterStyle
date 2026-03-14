using System.Text.Json;
using InterStyle.Curtains.Application;
using InterStyle.Curtains.Application.Queries;
using InterStyle.Curtains.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace InterStyle.Curtains.Infrastructure.Persistence;

/// <summary>
/// Caching decorator for <see cref="ICurtainQueries"/>.
/// Caches <see cref="GetAllAsync"/> results in Redis using versioned keys.
/// Implements <see cref="ICurtainCacheInvalidator"/> to bump the version on write operations.
/// </summary>
public sealed class CachedCurtainsQueries(
    ICurtainQueries inner,
    IDistributedCache cache,
    IOptions<CurtainsCacheOptions> options)
    : ICurtainQueries, ICurtainCacheInvalidator
{
    private const string VersionKey = "curtains:all:version";
    private const string CacheKeyPrefix = "curtains:all:";

    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(options.Value.DurationMinutes);

    /// <inheritdoc />
    public async Task<IReadOnlyList<CurtainDto>> GetAllAsync(Locale locale, CancellationToken cancellationToken = default)
    {
        var version = await cache.GetStringAsync(VersionKey, cancellationToken) ?? "0";
        var cacheKey = $"{CacheKeyPrefix}v{version}:{locale.Value}";

        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return JsonSerializer.Deserialize<List<CurtainDto>>(cached) ?? [];
        }

        var result = await inner.GetAllAsync(locale, cancellationToken);

        var json = JsonSerializer.Serialize(result);

        await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        }, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public Task<CurtainDto?> GetByIdAsync(Guid id, Locale locale, CancellationToken cancellationToken = default)
    {
        return inner.GetByIdAsync(id, locale, cancellationToken);
    }

    /// <inheritdoc />
    public async Task InvalidateAllCurtainsAsync(CancellationToken cancellationToken = default)
    {
        await cache.SetStringAsync(VersionKey, Guid.NewGuid().ToString("N"), cancellationToken);
    }
}
