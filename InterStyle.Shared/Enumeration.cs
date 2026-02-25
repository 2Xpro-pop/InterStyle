using System.Collections.Concurrent;
using System.Reflection;

namespace InterStyle.Shared;

/// <summary>
/// Base class for "smart enum" pattern used in DDD.
/// Allows richer behavior than C# enum while staying strongly-typed.
/// </summary>
public abstract class Enumeration : IComparable
{
    private static readonly ConcurrentDictionary<Type, object> AllItemsCache = new();

    protected Enumeration(int id, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Enumeration name is required.", nameof(name));
        }

        Id = id;
        Name = name;
    }

    public int Id { get; }

    public string Name { get; }

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType() == otherValue.GetType();
        var valueMatches = Id == otherValue.Id;

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => HashCode.Combine(GetType(), Id);

    public int CompareTo(object? other)
    {
        if (other is null)
        {
            return 1;
        }

        return Id.CompareTo(((Enumeration)other).Id);
    }

    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Enumeration? left, Enumeration? right) => !(left == right);

    /// <summary>
    /// Returns all public static fields declared on the derived type (declared-only).
    /// Cached per derived type for performance.
    /// </summary>
    public static IReadOnlyCollection<TEnumeration> GetAll<TEnumeration>()
        where TEnumeration : Enumeration
    {
        var derivedType = typeof(TEnumeration);

        var cached = AllItemsCache.GetOrAdd(derivedType, static type =>
        {
            var fields = type.GetFields(
                BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

            var items = fields
                .Select(static fieldInfo => fieldInfo.GetValue(null))
                .Cast<TEnumeration>()
                .ToArray();

            return items;
        });

        return (IReadOnlyCollection<TEnumeration>)cached;
    }

    /// <summary>
    /// Finds an Enumeration instance by id. Throws if not found.
    /// </summary>
    public static TEnumeration FromId<TEnumeration>(int id)
        where TEnumeration : Enumeration
    {
        var allItems = GetAll<TEnumeration>();
        var matchedItem = allItems.SingleOrDefault(item => item.Id == id);

        if (matchedItem is null)
        {
            throw new KeyNotFoundException(
                $"No {typeof(TEnumeration).Name} with Id '{id}' was found.");
        }

        return matchedItem;
    }

    /// <summary>
    /// Finds an Enumeration instance by name (case-insensitive by default). Throws if not found.
    /// </summary>
    public static TEnumeration FromName<TEnumeration>(string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        where TEnumeration : Enumeration
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        var allItems = GetAll<TEnumeration>();
        var matchedItem = allItems.SingleOrDefault(item => string.Equals(item.Name, name, comparison));

        if (matchedItem is null)
        {
            throw new KeyNotFoundException(
                $"No {typeof(TEnumeration).Name} with Name '{name}' was found.");
        }

        return matchedItem;
    }

    /// <summary>
    /// Tries to find an Enumeration instance by id.
    /// </summary>
    public static bool TryFromId<TEnumeration>(int id, out TEnumeration? value)
        where TEnumeration : Enumeration
    {
        var allItems = GetAll<TEnumeration>();
        value = allItems.SingleOrDefault(item => item.Id == id);
        return value is not null;
    }

    /// <summary>
    /// Tries to find an Enumeration instance by name.
    /// </summary>
    public static bool TryFromName<TEnumeration>(string name, out TEnumeration? value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        where TEnumeration : Enumeration
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            value = null;
            return false;
        }

        var allItems = GetAll<TEnumeration>();
        value = allItems.SingleOrDefault(item => string.Equals(item.Name, name, comparison));
        return value is not null;
    }

    public static bool operator <(Enumeration left, Enumeration right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Enumeration left, Enumeration right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Enumeration left, Enumeration right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Enumeration left, Enumeration right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }
}