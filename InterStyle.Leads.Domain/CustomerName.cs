using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Represents validated customer name.
/// </summary>
public readonly record struct CustomerName
{
    public string Value { get; }

    private CustomerName(string value)
    {
        Value = value;
    }

    public static CustomerName Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Customer name is required.", nameof(input));

        var normalized = input.Trim();

        if (normalized.Length < 2)
        {
            throw new ArgumentException("Customer name is too short.", nameof(input));
        }

        if (normalized.Length > 100)
        {
            throw new ArgumentException("Customer name is too long.", nameof(input));
        }

        return new CustomerName(normalized);
    }

    internal static CustomerName Rehydrate(string persisted)
    {
        if (string.IsNullOrWhiteSpace(persisted))
        {
            throw new InvalidOperationException("Corrupted data: CustomerName empty.");
        }

        return new CustomerName(persisted.Trim());
    }

    public override string ToString() => Value;

    public static implicit operator string(CustomerName name) => name.Value;

    public static explicit operator CustomerName(string value) => Create(value);
}