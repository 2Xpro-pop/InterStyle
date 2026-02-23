using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Raised when a new lead is created.
/// </summary>
public sealed record LeadCreatedDomainEvent(
    LeadId LeadId,
    DateTimeOffset OccurredAtUtc) : IDomainEvent;