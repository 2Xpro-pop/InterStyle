using Dapper;
using InterStyle.Curtains.Application.Queries;
using InterStyle.Curtains.Domain;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace InterStyle.Curtains.Infrastructure.Persistence;

/// <summary>
/// Dapper-based implementation of ICurtainQueries for CQRS read-side.
/// Fallback order: requested locale → default locale → any available translation.
/// </summary>
public sealed class CurtainsQueries(IConfiguration configuration) : ICurtainQueries
{
    private static readonly string DefaultLocale = Locale.Default.Value;

    private readonly string _connectionString = configuration.GetConnectionString("curtainsdb")
        ?? throw new InvalidOperationException("Connection string 'curtainsdb' not found.");

    /// <inheritdoc />
    public async Task<IReadOnlyList<CurtainDto>> GetAllAsync(Locale locale, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT DISTINCT ON (c.id)
                c.id          AS "Id",
                t.name        AS "Name",
                t.description AS "Description",
                t.locale      AS "Locale",
                c.picture_url AS "PictureUrl",
                c.preview_url AS "PreviewUrl"
            FROM curtains.curtains c
            INNER JOIN curtains.curtain_translations t ON t.curtain_id = c.id
            ORDER BY c.id,
                     (t.locale = @locale) DESC,
                     (t.locale = @defaultLocale) DESC,
                     t.locale
            """;

        var items = await connection.QueryAsync<CurtainDto>(sql, new { locale = locale.Value, defaultLocale = DefaultLocale });

        return [.. items];
    }

    /// <inheritdoc />
    public async Task<CurtainDto?> GetByIdAsync(Guid id, Locale locale, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        const string sql = """
            SELECT
                c.id          AS "Id",
                t.name        AS "Name",
                t.description AS "Description",
                t.locale      AS "Locale",
                c.picture_url AS "PictureUrl",
                c.preview_url AS "PreviewUrl"
            FROM curtains.curtains c
            INNER JOIN curtains.curtain_translations t ON t.curtain_id = c.id
            WHERE c.id = @id
            ORDER BY (t.locale = @locale) DESC,
                     (t.locale = @defaultLocale) DESC,
                     t.locale
            LIMIT 1
            """;

        return await connection.QuerySingleOrDefaultAsync<CurtainDto>(sql, new { id, locale = locale.Value, defaultLocale = DefaultLocale });
    }
}
