using System.Numerics;

namespace PanelCutting.Collections;

internal readonly record struct Interval<T>(T Min, T Max) where T : IComparable<T>, ISubtractionOperators<T, T, T>, IMinMaxValue<T>
{
    public T? Length => Min.CompareTo(Max) <= 0 ? Max - Min : default;
    public Interval<T> GrowMin(T value) => this with { Min = GetMin(Min, value) };
    public Interval<T> GrowMax(T value) => this with { Max = GetMax(Max, value) };
    public Interval<T> AddPoint(T value) => Union(GrowMin(value), GrowMax(value));
    public bool Contains(T value) => Min.CompareTo(value) <= 0 && value.CompareTo(Max) <= 0;
    public static Interval<T> Union(Interval<T> first, Interval<T> second) => 
        new(Min: GetMin(first.Min, second.Min), Max: GetMax(first.Max, second.Max));
    public static Interval<T> Create(T first, T second) => new(Min: GetMin(first, second), Max: GetMax(first, second));
    public static Interval<T> CreateEmpty() => new(Min: T.MaxValue, Max: T.MinValue);
    public static Interval<T> CreateFromPoint(T value) => new(Min: value, Max: value);
    public static Interval<T> CreateFromPoints(T first, T second) => new(Min: GetMin(first, second), Max: GetMax(first, second));
    private static T GetMin(T lhs, T rhs) => lhs.CompareTo(rhs) <= 0 ? lhs : rhs;
    private static T GetMax(T lhs, T rhs) => lhs.CompareTo(rhs) >= 0 ? lhs : rhs;
}