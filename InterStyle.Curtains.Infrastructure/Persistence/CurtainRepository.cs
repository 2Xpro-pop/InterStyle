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

    public async Task<ImmutableArray<Curtain>> GetCurtainsAsync()
    {
        var array = await _dbContext.Set<Curtain>().ToArrayAsync();

        return array.ToImmutableArrayUnsafe();
    }
}
