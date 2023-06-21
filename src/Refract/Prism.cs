namespace Refract;

/// <summary>
/// Provides an abstract view to manipulate a sum type <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">The source type to manipulate.</typeparam>
/// <typeparam name="TU">The projected type.</typeparam>
public readonly struct Prism<T, TU> {
    readonly Func<T, TU?> _get;
    readonly Action<T, TU>? _set;

    /// <summary>
    /// Initializes a new <see cref="Prism{T,TU}"/>.
    /// </summary>
    /// <param name="get">A function to read a <typeparamref name="T"/>.</param>
    /// <param name="set">An optional function to write a <typeparamref name="T"/>.</param>
    public Prism(Func<T, TU?> get, Action<T, TU>? set = null) {
        _get = get;
        _set = set;
    }

    public static implicit operator Func<T, TU?>(Prism<T, TU> instance) => instance._get;

    public static implicit operator Action<T, TU>(Prism<T, TU> instance) => instance._set ?? ((_, _) => { });
}

public static class Prism {
    /// <summary>
    /// Composes two <see cref="Prism{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TResult">The type of the output subject.</typeparam>
    /// <param name="prism1">The first <see cref="Prism{T,TU}"/> to look through.</param>
    /// <param name="prism2">The second <see cref="Prism{T,TU}"/> to look through.</param>
    /// <returns>A <see cref="Prism{T,TResult}"/>.</returns>
    public static Prism<T, TResult> Compose<T, TInter, TResult>(this Prism<T, TInter> prism1, Prism<TInter, TResult> prism2) =>
        new((Func<T, TResult?>)Delegate.Combine((Func<T, TInter?>)prism1, new Func<TInter?, TResult?>(maybe => maybe is {} subject ? ((Func<TInter, TResult?>)prism2)(subject) : default)));

    /// <summary>
    /// Composes a <see cref="Prism"/> and a <see cref="Lens"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TResult">The type of the output subject.</typeparam>
    /// <param name="prism">The <see cref="Prism"/> to look through first.</param>
    /// <param name="lens">The <see cref="Lens"/> to look through second.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<T, TResult> Compose<T, TInter, TResult>(this Prism<T, TInter> prism, Lens<TInter?, TResult> lens) =>
        new((Func<T?, TResult>)Delegate.Combine((Func<T, TInter?>)prism, (Func<TInter?, TResult>)lens));
}