namespace GtPrax.Domain.ValueObjects;

using System;
using System.Collections.Generic;
using System.Linq;

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left?.Equals(right!) != false;
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is not ValueObject other)
        {
            return false;
        }
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public static bool operator ==(ValueObject one, ValueObject two) => EqualOperator(one, two);

    public static bool operator !=(ValueObject one, ValueObject two) => !EqualOperator(one, two);

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var component in GetEqualityComponents())
        {
            hash.Add(component);
        }

        return hash.ToHashCode();
    }
}
