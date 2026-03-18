using System.Diagnostics;

namespace InterStyle.Reviews.Domain;

/// <summary>
/// Strongly-typed identifier for Review aggregate.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct ReviewId(Guid Value)
{
    /// <summary>
    /// Creates a new unique ReviewId.
    /// </summary>
    public static ReviewId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}