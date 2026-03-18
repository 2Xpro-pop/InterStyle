using System.Diagnostics;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Strongly-typed identifier for Curtain aggregate.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct CurtainId(Guid Value)
{
    /// <summary>
    /// Creates a new unique CurtainId.
    /// </summary>
    public static CurtainId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();
}