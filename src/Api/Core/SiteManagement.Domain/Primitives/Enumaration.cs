using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SiteManagement.Domain.Primitives;

public abstract class Enumaration<TEnum> : IEquatable<Enumaration<TEnum>>
    where TEnum : Enumaration<TEnum>
{
    private static readonly Dictionary<int, TEnum> Enumarations = CreateEnumarations();
    protected Enumaration(int value, string name) 
    {
        Value = value;
        Name = name;
    }

    public int Value { get; protected init; }
    public string Name { get; protected init; } = string.Empty;
    public static TEnum? FromValue(int value)
    {
        return Enumarations.TryGetValue(value,
                                        out TEnum? enumaration) ?
                                        enumaration :
                                        default;
    }
    public static TEnum? FromName(string name)
    {
        return Enumarations.Values
               .SingleOrDefault(e => e.Name == name);
    }
    public bool Equals(Enumaration<TEnum>? other)
    {
        if(other is null)
           return false;

        return GetType() == other.GetType() &&
               Value == other.Value;

    }
    public override bool Equals(object? obj)
    {
        return obj is Enumaration<TEnum> other &&
               Equals(other);

    }
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
    public override string ToString()
    {
        return Name;
    }
    private static Dictionary<int,TEnum> CreateEnumarations()
    {
        var enumerationType = typeof(TEnum);

        var fieldsForType = enumerationType
            .GetFields(
                        BindingFlags.Public |
                        BindingFlags.Static |
                        BindingFlags.FlattenHierarchy)
            .Where(fieldInfo =>
                   enumerationType.IsAssignableFrom(fieldInfo.FieldType))
            .Select(fieldInfo => 
                    (TEnum)fieldInfo.GetValue(default)!);

        return fieldsForType.ToDictionary(x => x.Value);
    }
}
