using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Shared;

/// <summary>
/// Unit of Work abstraction.
/// Implemented by EF DbContext or Dapper transaction wrapper.
/// </summary>
public interface IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}