using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InterStyle.Leads.Domain;

/// <summary>
/// Represents validated phone number value object (Kyrgyzstan-first).
/// Stored as digits only (e.g., 996700123456).
/// </summary>
public readonly partial record struct PhoneNumber
{
    private static readonly Regex DigitsOnly = MyRegex();

    public string Value { get; }

    private PhoneNumber(string normalizedDigits)
    {
        // Minimal invariant: never store empty
        if (string.IsNullOrWhiteSpace(normalizedDigits))
            throw new ArgumentException("Phone number cannot be empty.", nameof(normalizedDigits));

        Value = normalizedDigits;
    }

    /// <summary>
    /// Factory method with validation and normalization.
    /// Accepts:
    /// - +996700123456
    /// - 996700123456
    /// - 0700123456 / 0555123456 (converted to 996700123456 / 996555123456)
    /// </summary>
    public static PhoneNumber Create(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Phone number is required.", nameof(input));
        }

        var digits = NormalizeToDigits(input);

        var normalizedKg = NormalizeKyrgyzstan(digits);

        // Final sanity check: Kyrgyzstan numbers are typically 12 digits: 996 + 9 digits
        // Example: 996700123456 (12 digits total)
        if (normalizedKg.Length != 12 || !normalizedKg.StartsWith("996", StringComparison.Ordinal))
            throw new ArgumentException("Invalid Kyrgyzstan phone number.", nameof(input));

        return new PhoneNumber(normalizedKg);
    }

    /// <summary>
    /// Used only by persistence when data is already normalized.
    /// Still validates invariants to detect corrupted data.
    /// </summary>
    internal static PhoneNumber Rehydrate(string persistedDigits)
    {
        if (string.IsNullOrWhiteSpace(persistedDigits))
        {
            throw new InvalidOperationException("Corrupted phone number in database.");
        }

        // Expect already normalized KG digits
        if (persistedDigits.Length != 12 || !persistedDigits.StartsWith("996", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Corrupted phone number format in database.");
        }

        return new PhoneNumber(persistedDigits);
    }

    private static string NormalizeToDigits(string input)
        => DigitsOnly.Replace(input, "");

    private static string NormalizeKyrgyzstan(string digits)
    {
        // If user typed local format with leading 0, e.g. 0700123456 -> 996700123456
        if (digits.Length == 10 && digits.StartsWith("0", StringComparison.Ordinal))
        {
            // remove leading 0, prefix 996
            return "996" + digits[1..];
        }

        // If user typed without +, e.g. 996700123456
        if (digits.Length == 12 && digits.StartsWith("996", StringComparison.Ordinal))
        {
            return digits;
        }

        // If user typed already in E.164 digits but with plus removed by normalization: +996700123456 -> 996700123456
        if (digits.Length == 12 && digits.StartsWith("996", StringComparison.Ordinal))
        {
            return digits;
        }

        // If user typed 9-digit national number without prefix (700123456) — decide if you want to accept it.
        // I recommend rejecting to avoid ambiguity:
        // if (digits.Length == 9) return "996" + digits;

        throw new ArgumentException("Unsupported phone number format.", nameof(digits));
    }

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phone) => phone.Value;

    public static explicit operator PhoneNumber(string value) => Create(value);

    [GeneratedRegex(@"\D", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}