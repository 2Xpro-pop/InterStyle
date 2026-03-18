using InterStyle.Reviews.Domain;
using InterStyle.Shared;
using Microsoft.EntityFrameworkCore;

namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// Repository implementation for Review aggregate.
/// </summary>
public sealed class ReviewRepository(ReviewsDbContext dbContext) : IReviewRepository
{
    private readonly ReviewsDbContext _dbContext = dbContext
        ?? throw new ArgumentNullException(nameof(dbContext));

    /// <inheritdoc />
    public IUnitOfWork UnitOfWork => _dbContext;

    /// <inheritdoc />
    public async Task<Review?> GetByIdAsync(ReviewId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<Review>()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Review review, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<Review>().AddAsync(review, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Review review)
    {
        _dbContext.Set<Review>().Update(review);
    }
}
