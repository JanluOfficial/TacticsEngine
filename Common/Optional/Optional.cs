using System.Collections;

namespace CaptainCoder.Functional;

public abstract record Optional<T> : IEnumerable<T>
{
    public static implicit operator Optional<T>(T a) => new Some<T>(a);
    public abstract IEnumerator<T> GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
public sealed record Some<T>(T Value) : Optional<T>
{
    public override IEnumerator<T> GetEnumerator() { yield return Value; }
}

public sealed record None<T> : Optional<T>
{
    public override IEnumerator<T> GetEnumerator() { yield break; }
}

public static class OptionalExtensions
{
    public static Optional<T> Some<T>(this T value) => new Some<T>(value);
    public static Optional<T> None<T>() => new None<T>();
    public static Optional<T> SomeOrNone<T>(this T? value) => value switch
    {
        T val => Some(val),
        _ => None<T>(),
    };

    public static bool IsSome<T>(this Optional<T> optional) => optional is Some<T>;
    public static bool IsNone<T>(this Optional<T> optional) => optional is None<T>;

    public static void Invoke<T>(this Optional<T> optional, Action<T> toApply)
    {
        if (optional is Some<T>(T value)) { toApply.Invoke(value); }
    }

    public static void Invoke<T1, T2>(this Optional<T1> optional, Func<T1, T2> toInvoke) => optional.Invoke<T1>(f => toInvoke.Invoke(f));

    public static Optional<T> Filter<T>(this Optional<T> optional, Predicate<T> pred) => optional switch
    {
        Some<T>(T val) when pred(val) is true => optional,
        _ => None<T>(),
    };

    public static Optional<ReturnVal> Map<TVal, ReturnVal>(this Optional<TVal> optional, Func<TVal, ReturnVal> toApply) => optional switch
    {
        Some<TVal>(TVal value) => new Some<ReturnVal>(toApply.Invoke(value)),
        _ => None<ReturnVal>(),
    };

    public static Optional<ReturnVal> FlatMap<TVal, ReturnVal>(this Optional<TVal> optional, Func<TVal, Optional<ReturnVal>> toApply) => optional switch
    {
        Some<TVal>(TVal value) => toApply.Invoke(value),
        _ => None<ReturnVal>(),
    };

    public static T Value<T>(this Optional<T> optional) => optional switch
    {
        Some<T>(T val) => val,
        _ => throw new InvalidOperationException($"{optional} does not have a value.")
    };

    public static T ValueOr<T>(this Optional<T> optional, T @default) => optional switch
    {
        Some<T>(T value) => value,
        _ => @default,
    };

    public static T? ValueOrDefault<T>(this Optional<T> optional) => optional switch
    {
        Some<T>(T value) => value,
        _ => default,
    };

}