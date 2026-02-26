using InterStyle.Curtains.Domain;
using InterStyle.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace InterStyle.Curtains.Infrastructure.Persistence;

public sealed class CurtainRepository(CurtainsDbContext dbContext): ICurtainRepository
{
    private readonly CurtainsDbContext _dbContext = dbContext;

    public IUnitOfWork UnitOfWork => _dbContext;

    public async Task<ImmutableArray<Curtain>> GetCurtainsAsync(CancellationToken cancellationToken = default)
    {
        var array = await _dbContext.Set<Curtain>().ToArrayAsync(cancellationToken);

        return array.ToImmutableArrayUnsafe();
    }

    public async Task<Curtain?> AddAsync(Curtain curtain, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(curtain);
        var entry = await _dbContext.Set<Curtain>().AddAsync(curtain, cancellationToken);
        return entry.Entity;
    }
}
