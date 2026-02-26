using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Reviews.Domain;

/// <summary>
/// Represents validated review comment.
/// Immutable.
/// </summary>
public readonly record struct ReviewComment
{
    public string Value { get; }

    private ReviewComment(string value)
    {
        Value = value;
    }

    public static ReviewComment Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Comment is required.", nameof(input));
        }

        var normalized = Normalize(input);

        if (normalized.Length < 5)
        {
            throw new ArgumentException("Comment is too short.", nameof(input));
        }

        if (normalized.Length > 2000)
        {
            throw new ArgumentException("Comment is too long.", nameof(input));
        }

        return new ReviewComment(normalized);
    }

    internal static ReviewComment Rehydrate(string persisted)
    {
        if (string.IsNullOrWhiteSpace(persisted))
        {
            throw new InvalidOperationException("Corrupted data: ReviewComment empty.");
        }

        return new ReviewComment(persisted);
    }

    private static string Normalize(string input)
        => input.Trim();

    public override string ToString() => Value;

    public static implicit operator string(ReviewComment comment) => comment.Value;

    public static explicit operator ReviewComment(string value) => Create(value);
}