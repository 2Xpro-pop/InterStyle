using Dapper;
using InterStyle.Curtains.Application.Queries;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InterStyle.Curtains.Infrastructure.Persistence;

/// <summary>
/// Dapper-based implementation of ICurtainQueries for CQRS read-side.
/// </summary>
public sealed class CurtainsQueries(IConfiguration configuration) : ICurtainQueries
{
    private readonly string _connectionString = configuration.GetConnectionString("curtainsdb")
        ?? throw new InvalidOperationException("Connection string 'curtainsdb' not found.");

    /// <inheritdoc />
    public async Task<IReadOnlyList<CurtainDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT
                id AS "Id",
                name AS "Name",
                description AS "Description",
                picture_url AS "PictureUrl",
                preview_url AS "PreviewUrl"
            FROM curtains.curtains
            ORDER BY name
            """;

        var items = await connection.QueryAsync<CurtainDto>(sql);

        return [.. items];
    }

    /// <inheritdoc />
    public async Task<CurtainDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT
                id AS "Id",
                name AS "Name",
                description AS "Description",
                picture_url AS "PictureUrl",
                preview_url AS "PreviewUrl"
            FROM curtains.curtains
            WHERE id = @id
            """;

        return await connection.QuerySingleOrDefaultAsync<CurtainDto>(sql, new { id });
    }
}
