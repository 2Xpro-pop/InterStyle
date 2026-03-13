using System.Diagnostics;
using System.Text.RegularExpressions;

namespace InterStyle.Curtains.Domain;

/// <summary>
/// Represents a BCP 47 language tag (e.g. "en", "ru", "ru-RU").
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct Locale
{
    private static readonly Regex BcpPattern = new(@"^[a-z]{2,3}(-[a-z]{2,8})*$", RegexOptions.Compiled);

    /// <summary>
    /// The default catalog locale used as fallback.
    /// </summary>
    public static readonly Locale Default = new("ru-ru");

    public string Value { get; }

    private Locale(string value)
    {
        Value = value;
    }

    public static Locale Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Locale is required.", nameof(input));
        }

        var normalized = input.Trim().ToLowerInvariant();

        if (!BcpPattern.IsMatch(normalized))
        {
            throw new ArgumentException($"'{input}' is not a valid BCP 47 language tag.", nameof(input));
        }

        return new Locale(normalized);
    }

    internal static Locale Rehydrate(string persisted)
    {
        if (string.IsNullOrWhiteSpace(persisted))
        {
            throw new InvalidOperationException("Corrupted data: Locale empty.");
        }

        return new Locale(persisted.Trim().ToLowerInvariant());
    }

    public override string ToString() => Value;

    public static implicit operator string(Locale locale) => locale.Value;

    public static explicit operator Locale(string value) => Create(value);
}
