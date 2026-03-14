using System.Text.Json;
using InterStyle.Reviews.Application;
using InterStyle.Reviews.Application.Queries;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// Caching decorator for <see cref="IReviewQueries"/>.
/// Caches up to <see cref="ReviewsCacheOptions.MaxCachedItems"/> approved reviews in a single Redis entry.
/// Requests beyond that range fall through to the database.
/// Implements <see cref="IReviewCacheInvalidator"/> to bump the version on write operations.
/// </summary>
public sealed class CachedReviewQueries(
    IReviewQueries inner,
    IDistributedCache cache,
    IOptions<ReviewsCacheOptions> options)
    : IReviewQueries, IReviewCacheInvalidator
{
    private const string VersionKey = "reviews:approved:version";
    private const string CacheKeyPrefix = "reviews:approved:";

    private readonly int _maxCachedItems = options.Value.MaxCachedItems;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(options.Value.DurationMinutes);

    /// <inheritdoc />
    public async Task<PagedResult<ReviewDto>> GetApprovedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var offset = (page - 1) * pageSize;

        if (offset + pageSize > _maxCachedItems)
        {
            return await inner.GetApprovedAsync(page, pageSize, cancellationToken);
        }

        var snapshot = await GetOrLoadSnapshotAsync(cancellationToken);

        var slice = snapshot.Items
            .Skip(offset)
            .Take(pageSize)
            .ToList();

        return new PagedResult<ReviewDto>(
            Items: slice,
            Page: page,
            PageSize: pageSize,
            TotalCount: snapshot.TotalCount);
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

    /// <inheritdoc />
    public async Task InvalidateApprovedReviewsAsync(CancellationToken cancellationToken = default)
    {
        await cache.SetStringAsync(VersionKey, Guid.NewGuid().ToString("N"), cancellationToken);
    }

    private async Task<ApprovedReviewsSnapshot> GetOrLoadSnapshotAsync(CancellationToken cancellationToken)
    {
        var version = await cache.GetStringAsync(VersionKey, cancellationToken) ?? "0";
        var cacheKey = $"{CacheKeyPrefix}v{version}";

        var cached = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cached is not null)
        {
            return JsonSerializer.Deserialize<ApprovedReviewsSnapshot>(cached)!;
        }

        var result = await inner.GetApprovedAsync(page: 1, pageSize: _maxCachedItems, cancellationToken);

        var snapshot = new ApprovedReviewsSnapshot(result.Items, result.TotalCount);
        var json = JsonSerializer.Serialize(snapshot);

        await cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheDuration
        }, cancellationToken);

        return snapshot;
    }

    private sealed record ApprovedReviewsSnapshot(
        IReadOnlyList<ReviewDto> Items,
        long TotalCount);
}
