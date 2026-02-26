using InterStyle.Reviews.Domain;
using InterStyle.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace InterStyle.Reviews.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for the Reviews bounded context.
/// Implements IUnitOfWork for write-side operations.
/// </summary>
public sealed class ReviewsDbContext(DbContextOptions<ReviewsDbContext> options, IMediator mediator)
    : DbContext(options), IUnitOfWork
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private IDbContextTransaction? _currentTransaction;

    /// <summary>
    /// Gets a value indicating whether there is an active transaction.
    /// </summary>
    public bool HasActiveTransaction => _currentTransaction is not null;

    /// <summary>
    /// Gets the current transaction if any.
    /// </summary>
    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("reviews");
        modelBuilder.ApplyConfiguration(new ReviewEntityTypeConfiguration());
    }

    /// <summary>
    /// Saves entities and dispatches domain events.
    /// </summary>
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);
        _ = await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <inheritdoc />
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <returns>The transaction if started; null if one already exists.</returns>
    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            return null;
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        return _currentTransaction;
    }

    /// <summary>
    /// Commits the specified transaction.
    /// </summary>
    /// <param name="transaction">The transaction to commit.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CommitTransactionAsync(IDbContextTransaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction != _currentTransaction)
        {
            throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current.");
        }

        try
        {
            _ = await SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction!.Dispose();
                _currentTransaction = null;
            }
        }
    }

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (HasActiveTransaction)
            {
                _currentTransaction!.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
