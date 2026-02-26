using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Curtains.Domain;

public readonly record struct Description
{
    public string Value { get; }

    private Description(string value)
    {
        Value = value;
    }

    public static Description Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Description is required.", nameof(input));
        }
        
        var normalized = input.Trim();
        
        if (normalized.Length > 500)
        {
            throw new ArgumentException("Description is too long.", nameof(input));
        }

        return new Description(normalized);
    }

    internal static Description Rehydrate(string persisted)
    {
        if (string.IsNullOrWhiteSpace(persisted))
        {
            throw new InvalidOperationException("Corrupted data: Description empty.");
        }
        return new Description(persisted.Trim());
    }

    public override string ToString() => Value;
    public static implicit operator string(Description description) => description.Value;
    public static explicit operator Description(string value) => Create(value);
}
