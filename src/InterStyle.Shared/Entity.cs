using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace InterStyle.Shared;

/// <summary>
/// Base class for entities with strongly-typed identifiers.
/// </summary>
public abstract class Entity<TId>
    where TId : notnull
{
    private int? _requestedHashCode;

    public TId Id { get; protected set; } = default!;

    protected Entity()
    {
    }

    protected Entity(TId id)
    {
        Id = id;
    }

    public bool IsTransient() => EqualityComparer<TId>.Default.Equals(Id, default!);

    public override bool Equals(object? obj)
    {
        if (obj is null || obj is not Entity<TId> other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (IsTransient() || other.IsTransient())
        {
            return false;
        }

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        if (IsTransient())
        {
            return base.GetHashCode();
        }

        _requestedHashCode ??= HashCode.Combine(GetType(), Id);
        return _requestedHashCode.Value;
    }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);
}