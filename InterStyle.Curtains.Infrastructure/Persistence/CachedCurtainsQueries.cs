using System.Text.Json;
using InterStyle.Curtains.Application.Queries;
using InterStyle.Curtains.Domain;
using Microsoft.Extensions.Caching.Distributed;

namespace InterStyle.Curtains.Infrastructure.Persistence;

/// <summary>
/// Caching decorator for <see cref="ICurtainQueries"/>.
/// Caches <see cref="GetAllAsync"/> results in Redis; other methods delegate directly.
/// </summary>
public sealed class CachedCurtainsQueries(ICurtainQueries inner, IDistributedCache cache) : ICurtainQueries
{
    private const string CacheKeyPrefix = "curtains:all:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    /// <inheritdoc />
    public async Task<IReadOnlyList<CurtainDto>> GetAllAsync(Locale locale, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{locale.Value}";

        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return JsonSerializer.Deserialize<List<CurtainDto>>(cached) ?? [];
        }

        var result = await inner.GetAllAsync(locale, cancellationToken);

        var json = JsonSerializer.Serialize(result);

        await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        }, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public Task<CurtainDto?> GetByIdAsync(Guid id, Locale locale, CancellationToken cancellationToken = default)
    {
        return inner.GetByIdAsync(id, locale, cancellationToken);
    }
}
