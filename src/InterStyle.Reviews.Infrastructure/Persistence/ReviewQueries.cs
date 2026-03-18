using Dapper;
using InterStyle.Reviews.Application.Queries;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// Dapper-based implementation of IReviewQueries for CQRS read-side.
/// </summary>
public sealed class ReviewQueries(IConfiguration configuration) : IReviewQueries
{
    private readonly string _connectionString = configuration.GetConnectionString("reviewsdb")
        ?? throw new InvalidOperationException("Connection string 'reviewsdb' not found.");

    /// <inheritdoc />
    public async Task<PagedResult<ReviewDto>> GetApprovedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be at least 1.");
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 100.");
        }

        await using var connection = new NpgsqlConnection(_connectionString);

        var offset = (page - 1) * pageSize;

        const string countSql = """
            SELECT COUNT(*)
            FROM reviews.reviews
            WHERE is_approved = true
            """;

        const string dataSql = """
            SELECT
                id AS "Id",
                customer_name AS "CustomerName",
                rating AS "Rating",
                comment AS "Comment",
                is_approved AS "IsApproved",
                created_at_utc AS "CreatedAtUtc",
                approved_at_utc AS "ApprovedAtUtc"
            FROM reviews.reviews
            WHERE is_approved = true
            ORDER BY created_at_utc DESC
            LIMIT @pageSize OFFSET @offset
            """;

        var totalCount = await connection.ExecuteScalarAsync<long>(countSql);

        var items = await connection.QueryAsync<ReviewDto>(
            dataSql,
            new { pageSize, offset });

        return new PagedResult<ReviewDto>(
            Items: items.ToList(),
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ReviewDto>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT
                id AS "Id",
                customer_name AS "CustomerName",
                rating AS "Rating",
                comment AS "Comment",
                is_approved AS "IsApproved",
                created_at_utc AS "CreatedAtUtc",
                approved_at_utc AS "ApprovedAtUtc"
            FROM reviews.reviews
            WHERE is_approved = false
            ORDER BY created_at_utc ASC
            """;

        var items = await connection.QueryAsync<ReviewDto>(sql);

        return items.ToList();
    }

    /// <inheritdoc />
    public async Task<ReviewStatistics> GetStatisticsAsync(
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken = default)
    {
        if (fromUtc >= toUtc)
        {
            throw new ArgumentException("fromUtc must be earlier than toUtc.");
        }

        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT
                COUNT(*) AS "Total",
                COALESCE(AVG(rating), 0) AS "AverageRating",
                COUNT(*) FILTER (WHERE rating = 1) AS "Count1",
                COUNT(*) FILTER (WHERE rating = 2) AS "Count2",
                COUNT(*) FILTER (WHERE rating = 3) AS "Count3",
                COUNT(*) FILTER (WHERE rating = 4) AS "Count4",
                COUNT(*) FILTER (WHERE rating = 5) AS "Count5"
            FROM reviews.reviews
            WHERE created_at_utc >= @fromUtc
              AND created_at_utc < @toUtc
            """;

        return await connection.QuerySingleAsync<ReviewStatistics>(
            sql,
            new { fromUtc, toUtc });
    }
}
