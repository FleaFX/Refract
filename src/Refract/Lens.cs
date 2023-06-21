namespace Refract;

/// <summary>
/// Provides an abstract view to manipulate a <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The source type to manipulate/</typeparam>
/// <typeparam name="TU">The projected type.</typeparam>
public readonly struct Lens<T, TU> {
    readonly Func<T, TU> _get;
    readonly Action<T, TU>? _set;

    /// <summary>
    /// Initializes a new <see cref="Lens{T,TU}"/>.
    /// </summary>
    /// <param name="get">A function to read a <typeparamref name="T"/>.</param>
    /// <param name="set">An optional function to write a <typeparamref name="T"/>.</param>
    public Lens(Func<T, TU> get, Action<T, TU>? set = null) {
        _get = get;
        _set = set;
    }

    public static implicit operator Func<T, TU>(Lens<T, TU> instance) => instance._get;

    public static implicit operator Action<T, TU>(Lens<T, TU> instance) => instance._set ?? ((_, _) => { });
}

public static class Lens {
    /// <summary>
    /// Composes two <see cref="Lens{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TResult">The type of the output subject.</typeparam>
    /// <param name="lens1">The first <see cref="Lens{T,TU}"/> to look through.</param>
    /// <param name="lens2">The second <see cref="Lens{T,TU}"/> to look through.</param>
    /// <returns>A <see cref="Lens{T,TResult}"/>.</returns>
    public static Lens<T, TResult> Compose<T, TInter, TResult>(this Lens<T, TInter> lens1, Lens<TInter, TResult> lens2) =>
        new((Func<T, TResult>)Delegate.Combine((Func<T, TInter>)lens1, (Func<TInter, TResult>)lens2));

    /// <summary>
    /// Composes a <see cref="Lens{T,TU}"/> and a <see cref="Prism{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TResult">The type of the output subject.</typeparam>
    /// <param name="lens">The <see cref="Lens{T,TU}"/> to look through first.</param>
    /// <param name="prism">The <see cref="Prism{T,TU}"/> to look through second.</param>
    /// <returns>A <see cref="Prism{T,TU}"/>.</returns>
    public static Prism<T, TResult> Compose<T, TInter, TResult>(this Lens<T, TInter> lens, Prism<TInter, TResult> prism) =>
        new((Func<T, TResult?>)Delegate.Combine((Func<T, TInter>)lens, (Func<TInter, TResult?>)prism));
}
