using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Raised when lead status changes.
/// </summary>
public sealed record LeadStatusChangedDomainEvent(
    LeadId LeadId,
    int FromStatusId,
    int ToStatusId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;