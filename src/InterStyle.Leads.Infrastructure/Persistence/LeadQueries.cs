using InterStyle.Leads.Application.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;

namespace InterStyle.Leads.Infrastructure.Persistence;

public sealed class LeadQueries(IConfiguration config) : ILeadQueries
{
    private readonly string _connectionString = config.GetConnectionString("leadsdb")!;

    public async Task<LeadStatistics> GetStatisticsAsync(
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken ct = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);

        var sql = """
        SELECT
            COUNT(*) as "Total",
            COUNT(*) FILTER (WHERE status_id = 1) as "New",
            COUNT(*) FILTER (WHERE status_id = 2) as "InProgress",
            COUNT(*) FILTER (WHERE status_id = 3) as "Completed",
            COUNT(*) FILTER (WHERE status_id = 4) as "Canceled"
        FROM leads.leads
        WHERE created_at_utc >= @from
          AND created_at_utc < @to
        """;

        return await connection.QuerySingleAsync<LeadStatistics>(
            sql,
            new { from = fromUtc, to = toUtc });
    }
}