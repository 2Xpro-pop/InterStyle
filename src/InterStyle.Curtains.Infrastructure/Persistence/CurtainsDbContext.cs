using InterStyle.InfastructureShared;
using InterStyle.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace InterStyle.Curtains.Infrastructure.Persistence;

public sealed class CurtainsDbContext(DbContextOptions<CurtainsDbContext> options, IMediator mediator) : DbContext(options), IUnitOfWork
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    private IDbContextTransaction? _currentTransaction;

    public bool HasActiveTransaction => _currentTransaction is not null;

    public IDbContextTransaction? GetCurrentTransaction() => _currentTransaction;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("curtains");
        modelBuilder.ApplyConfiguration(new CurtainEntityTypeConfiguration());
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await _mediator.DispatchDomainEventsAsync(this);

        _ = await base.SaveChangesAsync(cancellationToken);
        return true;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    public async Task<IDbContextTransaction?> BeginTransactionAsync()
    {
        if (_currentTransaction is not null)
        {
            return null;
        }

        _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
        return _currentTransaction;
    }

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
