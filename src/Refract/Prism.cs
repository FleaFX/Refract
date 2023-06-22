namespace Refract;

/// <summary>
/// Provides an abstract view to manipulate a sum type <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">The source type to manipulate.</typeparam>
/// <typeparam name="TU">The projected type.</typeparam>
public readonly struct Prism<T, TU> {
    readonly Func<T, TU?> _get;
    readonly Func<TU, T, T> _set;

    /// <summary>
    /// Initializes a new <see cref="Prism{T,TU}"/>.
    /// </summary>
    /// <param name="get">A function to read a <typeparamref name="T"/>.</param>
    /// <param name="set">An optional function to write a <typeparamref name="T"/>.</param>
    public Prism(Func<T, TU?> get, Func<TU, T, T>? set = null) {
        _get = get;
        _set = set ?? ((_, _) => throw new InvalidOperationException($"Cannot write a {typeof(TU)} to a {typeof(T)} using a readonly lens."));
    }

    /// <summary>
    /// Reads from the given <paramref name="subject"/>.
    /// </summary>
    /// <param name="subject">The source object to read from.</param>
    /// <returns>A <typeparamref name="TU?"/>.</returns>
    public TU? Get(T subject) => _get(subject);

    /// <summary>
    /// Writes to the given <paramref name="subject"/>.
    /// </summary>
    /// <param name="subject">The object to write to.</param>
    /// <param name="value">The value to write.</param>
    public T Set(TU value, T subject) => _set.Invoke(value, subject);

    /// <summary>
    /// Deconstructs a <see cref="Prism{T,TU}"/> to its getter and setter functions respectively.
    /// </summary>
    /// <param name="get">The getter function to deconstruct to.</param>
    /// <param name="set">The setter function to deconstruct to.</param>
    public void Deconstruct(out Func<T, TU?> get, out Func<TU, T, T> set) => (get, set) = (_get, _set);

    /// <summary>
    /// Implicitly casts the given <see cref="Prism{T,TU}"/> to a <see cref="Func{TResult}"/> that reads a <typeparamref name="T"/>.
    /// </summary>
    /// <param name="instance">The <see cref="Prism{T,TU}"/> to cast.</param>
    public static implicit operator Func<T, TU?>(Prism<T, TU> instance) => instance._get;

    /// <summary>
    /// Implicitly casts the given <see cref="Prism{T,TU}"/> to a <see cref="Action{T}"/> that writes to a <typeparamref name="T"/>.
    /// </summary>
    /// <param name="instance">The <see cref="Prism{T,TU}"/> to cast.</param>
    public static implicit operator Func<TU, T, T>(Prism<T, TU> instance) => instance._set;
}

public static class Prism {
    /// <summary>
    /// Composes two <see cref="Prism{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TU">The type of the output subject.</typeparam>
    /// <param name="prism1">The first <see cref="Prism{T,TU}"/> to look through.</param>
    /// <param name="prism2">The second <see cref="Prism{T,TU}"/> to look through.</param>
    /// <returns>A <see cref="Prism{T,TResult}"/>.</returns>
    public static Prism<T, TU> Compose<T, TInter, TU>(this Prism<T, TInter> prism1, Prism<TInter, TU> prism2) =>
        new (
            get: subject => 
                prism1.Get(subject) switch {
                    null => default,
                    var inter => prism2.Get(inter)
                },
            set: (value, subject) =>
                prism1.Get(subject) switch {
                    null => subject,
                    var inter => prism1.Set(prism2.Set(value, inter), subject)
                }
        );

    /// <summary>
    /// Composes a <see cref="Prism"/> and a <see cref="Lens"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TU">The type of the output subject.</typeparam>
    /// <param name="prism">The <see cref="Prism"/> to look through first.</param>
    /// <param name="lens">The <see cref="Lens"/> to look through second.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<T, TU> Compose<T, TInter, TU>(this Prism<T, TInter> prism, Lens<TInter?, TU> lens) =>
        new (
            get: subject => lens.Get(prism.Get(subject)),
            set: (value, subject) => prism.Set(lens.Set(value, prism.Get(subject))!, subject)
        );
}