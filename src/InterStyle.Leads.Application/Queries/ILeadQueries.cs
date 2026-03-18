using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Application.Queries;

public interface ILeadQueries
{
    public Task<LeadStatistics> GetStatisticsAsync(
        DateTimeOffset fromUtc,
        DateTimeOffset toUtc,
        CancellationToken cancellationToken = default);
}

public sealed record LeadStatistics(
    long Total,
    long New,
    long InProgress,
    long Completed,
    long Canceled);