using InterStyle.Shared;

namespace InterStyle.Reviews.Domain;

/// <summary>
/// Review aggregate root representing a customer review of the curtain salon service.
/// </summary>
public sealed class Review : AggregateRoot<ReviewId>
{
    /// <summary>
    /// Private constructor for EF Core materialization.
    /// </summary>
    private Review() { }

    private Review(
        ReviewId id,
        CustomerName customerName,
        Rating rating,
        ReviewComment comment,
        bool isApproved,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? approvedAtUtc)
        : base(id)
    {
        CustomerName = customerName;
        Rating = rating;
        Comment = comment;
        IsApproved = isApproved;
        CreatedAtUtc = createdAtUtc;
        ApprovedAtUtc = approvedAtUtc;
    }

    /// <summary>
    /// Gets the customer name who submitted the review.
    /// </summary>
    public CustomerName CustomerName { get; private set; }

    /// <summary>
    /// Gets the rating (1-5) given by the customer.
    /// </summary>
    public Rating Rating { get; private set; }

    /// <summary>
    /// Gets the review comment text.
    /// </summary>
    public ReviewComment Comment { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the review has been approved by admin.
    /// </summary>
    public bool IsApproved { get; private set; }

    /// <summary>
    /// Gets the timestamp when the review was submitted.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; private set; }

    /// <summary>
    /// Gets the timestamp when the review was approved (null if not approved).
    /// </summary>
    public DateTimeOffset? ApprovedAtUtc { get; private set; }

    /// <summary>
    /// Submits a new review.
    /// </summary>
    /// <param name="customerName">The customer name.</param>
    /// <param name="rating">The rating (1-5).</param>
    /// <param name="comment">The review comment.</param>
    /// <param name="createdAtUtc">The timestamp of submission.</param>
    /// <returns>A new Review aggregate in pending state.</returns>
    /// <exception cref="ArgumentException">Thrown when customerName or comment is invalid.</exception>
    public static Review Submit(
        CustomerName customerName,
        Rating rating,
        ReviewComment comment,
        DateTimeOffset createdAtUtc)
    {
        if (customerName == default)
        {
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        }

        if (rating == default)
        {
            throw new ArgumentException("Rating is required.", nameof(rating));
        }

        if (comment == default)
        {
            throw new ArgumentException("Comment is required.", nameof(comment));
        }

        var review = new Review(
            id: ReviewId.New(),
            customerName: customerName,
            rating: rating,
            comment: comment,
            isApproved: false,
            createdAtUtc: createdAtUtc,
            approvedAtUtc: null);

        review.AddDomainEvent(new ReviewSubmittedDomainEvent(review.Id, createdAtUtc));

        return review;
    }

    /// <summary>
    /// Approves the review for public display.
    /// </summary>
    /// <param name="approvedAtUtc">The timestamp of approval.</param>
    /// <exception cref="InvalidOperationException">Thrown when review is already approved.</exception>
    public void Approve(DateTimeOffset approvedAtUtc)
    {
        if (IsApproved)
        {
            throw new InvalidOperationException("Review is already approved.");
        }

        IsApproved = true;
        ApprovedAtUtc = approvedAtUtc;

        AddDomainEvent(new ReviewApprovedDomainEvent(Id, approvedAtUtc));
    }

    /// <summary>
    /// Rehydrates the aggregate from persistence without raising domain events.
    /// Should be used only by repositories.
    /// </summary>
    internal static Review Rehydrate(
        ReviewId id,
        CustomerName customerName,
        Rating rating,
        ReviewComment comment,
        bool isApproved,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? approvedAtUtc)
    {
        if (id == default)
        {
            throw new InvalidOperationException("Corrupted data: ReviewId is empty.");
        }

        return new Review(
            id: id,
            customerName: customerName,
            rating: rating,
            comment: comment,
            isApproved: isApproved,
            createdAtUtc: createdAtUtc,
            approvedAtUtc: approvedAtUtc);
    }
}
