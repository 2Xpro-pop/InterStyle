using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Curtains.Domain;

public readonly record struct PictureUrl
{
    public string Value { get; }

    private PictureUrl(string value)
    {
        Value = value;
    }

    public static PictureUrl Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Picture URL is required.", nameof(input));
        }
        
        var normalized = input.Trim();
        
        if (!Uri.IsWellFormedUriString(normalized, UriKind.Absolute))
        {
            throw new ArgumentException("Invalid URL format.", nameof(input));
        }

        return new PictureUrl(normalized);
    }
    
    internal static PictureUrl Rehydrate(string persisted)
    {
        if (string.IsNullOrWhiteSpace(persisted))
        {
            throw new InvalidOperationException("Corrupted data: PictureUrl empty.");
        }
        return Create(persisted);
    }

    public override string ToString() => Value;
    public static implicit operator string(PictureUrl url) => url.Value;
    public static explicit operator PictureUrl(string value) => Create(value);
}
