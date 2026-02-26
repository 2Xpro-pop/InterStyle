namespace InterStyle.Reviews.Domain;

/// <summary>
/// Value object representing a review rating from 1 to 5.
/// Immutable and validated.
/// </summary>
public readonly record struct Rating
{
    /// <summary>
    /// Gets the rating value (1-5).
    /// </summary>
    public int Value { get; }

    private Rating(int value) => Value = value;

    /// <summary>
    /// Creates a new Rating with validation.
    /// </summary>
    /// <param name="value">Rating value (must be between 1 and 5).</param>
    /// <returns>Validated Rating value object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not between 1 and 5.</exception>
    public static Rating Create(int value)
    {
        if (value is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Rating must be between 1 and 5.");
        }

        return new Rating(value);
    }

    /// <summary>
    /// Rehydrates Rating from persisted data without validation.
    /// Should be used only by repositories.
    /// </summary>
    internal static Rating Rehydrate(int persisted)
    {
        if (persisted is < 1 or > 5)
        {
            throw new InvalidOperationException($"Corrupted data: Rating {persisted} is invalid.");
        }

        return new Rating(persisted);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(Rating rating) => rating.Value;

    public static explicit operator Rating(int value) => Create(value);
}