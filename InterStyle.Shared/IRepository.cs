using System;
using System.Collections.Generic;
using System.Text;

namespace InterStyle.Shared;


/// <summary>
/// Generic repository contract.
/// Enforces the rule: repository can target only aggregate roots.
/// </summary>
public interface IRepository<TAggregateRoot>
    where TAggregateRoot : IAggregateRoot
{
    public IUnitOfWork UnitOfWork { get; }
}