using Refit;

namespace AdminPanel.Services;

public interface ILeadsApi
{
    [Get("/api/leads/statistics")]
    Task<LeadStatisticsDto> GetStatisticsAsync(DateTimeOffset? fromUtc = null, DateTimeOffset? toUtc = null);
}

public sealed record LeadStatisticsDto(
    long Total,
    long New,
    long InProgress,
    long Completed,
    long Canceled);
