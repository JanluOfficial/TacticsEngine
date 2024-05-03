public abstract record Optional<T>
{
    public static implicit operator Optional<T>(T a) => new Some<T>(a);
}
public sealed record Some<T>(T Value) : Optional<T>;
public sealed record None<T> : Optional<T>;

public static class OptionalExtensions
{
    public static Optional<T> Some<T>(this T value) => new Some<T>(value);
    public static Optional<T> None<T>() => new None<T>();

    public static bool HasValue<T>(this Optional<T> optional) => optional is Some<T>;

    public static T Value<T>(this Optional<T> optional) => optional switch
    {
        Some<T>(T val) => val,
        _ => throw new InvalidOperationException($"{optional} does not have a value.")
    };

    public static void Apply<T>(this Optional<T> optional, Action<T> toApply)
    {
        if (optional is Some<T> some) { toApply.Invoke(some.Value); }
    }

    public static Optional<T> Where<T>(this Optional<T> optional, Predicate<T> pred) => optional switch
    {
        Some<T>(T val) when pred(val) is true => optional,
        _ => None<T>(),
    };

    public static Optional<ReturnVal> Select<TVal, ReturnVal>(this Optional<TVal> optional, Func<TVal, ReturnVal> toApply) => optional switch
    {
        Some<TVal>(TVal value) => new Some<ReturnVal>(toApply.Invoke(value)),
        _ => None<ReturnVal>(),
    };

    public static Optional<ReturnVal> SelectMany<TVal, ReturnVal>(this Optional<TVal> optional, Func<TVal, Optional<ReturnVal>> toApply) => optional switch
    {
        Some<TVal>(TVal value) => toApply.Invoke(value),
        _ => None<ReturnVal>(),
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