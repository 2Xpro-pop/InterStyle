using InterStyle.Shared;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace InterStyle.Curtains.Domain;

public interface ICurtainRepository: IRepository<Curtain>
{
    public Task<ImmutableArray<Curtain>> GetCurtainsAsync(CancellationToken cancellationToken = default);

    public Task<Curtain?> AddAsync(Curtain curtain, CancellationToken cancellationToken = default);
}
