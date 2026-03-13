using System.Text.Json;
using InterStyle.Reviews.Application.Queries;
using Microsoft.Extensions.Caching.Distributed;

namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// Caching decorator for <see cref="IReviewQueries"/>.
/// Caches <see cref="GetApprovedAsync"/> results in Redis; other methods delegate directly.
/// </summary>
public sealed class CachedReviewQueries(IReviewQueries inner, IDistributedCache cache) : IReviewQueries
{
    private const string CacheKeyPrefix = "reviews:approved:";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(2);

    /// <inheritdoc />
    public async Task<PagedResult<ReviewDto>> GetApprovedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{CacheKeyPrefix}{page}:{pageSize}";

        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return JsonSerializer.Deserialize<PagedResult<ReviewDto>>(cached)!;
        }

        var result = await inner.GetApprovedAsync(page, pageSize, cancellationToken);

        var json = JsonSerializer.Serialize(result);

        await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        }, cancellationToken);

        return result;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ReviewDto>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        return inner.GetPendingAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<ReviewStatistics> GetStatisticsAsync(
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken = default)
    {
        return inner.GetStatisticsAsync(fromUtc, toUtc, cancellationToken);
    }
}
