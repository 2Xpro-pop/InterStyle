using System.Diagnostics;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Strongly-typed identifier for Lead aggregate.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct LeadId(Guid Value)
{
    public static LeadId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}