﻿namespace Refract;

/// <summary>
/// Provides an abstract view to manipulate a <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The source type to manipulate/</typeparam>
/// <typeparam name="TU">The projected type.</typeparam>
public readonly struct Lens<T, TU> {
    readonly Func<T, TU> _get;
    readonly Func<TU, T, T> _set;

    /// <summary>
    /// Initializes a new <see cref="Lens{T,TU}"/>.
    /// </summary>
    /// <param name="get">A function to read a <typeparamref name="T"/>.</param>
    /// <param name="set">An optional function to write a <typeparamref name="T"/>.</param>
    public Lens(Func<T, TU> get, Func<TU, T, T>? set = null) {
        _get = get;
        _set = set ?? ((_, _) => throw new InvalidOperationException($"Cannot write a {typeof(TU)} to a {typeof(T)} using a readonly lens.")) ;
    }

    /// <summary>
    /// Reads from the given <paramref name="subject"/>.
    /// </summary>
    /// <param name="subject">The source object to read from.</param>
    /// <returns>A <typeparamref name="TU?"/>.</returns>
    public TU Get(T subject) => _get(subject);

    /// <summary>
    /// Writes to the given <paramref name="subject"/>.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="subject">The object to write to.</param>
    public T Set(TU value, T subject) => _set.Invoke(value, subject);

    /// <summary>
    /// Deconstructs a <see cref="Lens{T,TU}"/> to its getter and setter functions respectively.
    /// </summary>
    /// <param name="get">The getter function to deconstruct to.</param>
    /// <param name="set">The setter function to deconstruct to.</param>
    public void Deconstruct(out Func<T, TU> get, out Func<TU, T, T> set) => (get, set) = (_get, _set);

    /// <summary>
    /// Implicitly casts the given <see cref="Lens{T,TU}"/> to a <see cref="Func{TResult}"/> that reads a <typeparamref name="T"/>.
    /// </summary>
    /// <param name="instance">The <see cref="Lens{T,TU}"/> to cast.</param>
    public static implicit operator Func<T, TU>(Lens<T, TU> instance) => instance._get;

    /// <summary>
    /// Implicitly casts the given <see cref="Lens{T,TU}"/> to a <see cref="Action{T}"/> that writes to a <typeparamref name="T"/>.
    /// </summary>
    /// <param name="instance">The <see cref="Lens{T,TU}"/> to cast.</param>
    public static implicit operator Func<TU, T, T>(Lens<T, TU> instance) => instance._set;
}

public static class Lens {
    /// <summary>
    /// Uses the given <paramref name="lens"/> to get a <typeparamref name="TU"/> from the given <paramref name="subject"/> and passes it through the given <paramref name="map"/> function and then sets the result back on the subject.
    /// </summary>
    /// <typeparam name="T">The type of the subject.</typeparam>
    /// <typeparam name="TU">The type of the output subject of the lens.</typeparam>
    /// <param name="subject">The subject to map.</param>
    /// <param name="lens">The <see cref="Lens{T,TU}"/> to use.</param>
    /// <param name="map">The mapping function.</param>
    /// <returns>The subject with the mapping function applied.</returns>
    public static T GetAndSet<T, TU>(this Lens<T, TU> lens, T subject, Func<TU, TU> map) => lens.Set(map(lens.Get(subject)), subject);

    /// <summary>
    /// Composes two <see cref="Lens{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TU">The type of the output subject.</typeparam>
    /// <param name="lens1">The first <see cref="Lens{T,TU}"/> to look through.</param>
    /// <param name="lens2">The second <see cref="Lens{T,TU}"/> to look through.</param>
    /// <returns>A <see cref="Lens{T,TResult}"/>.</returns>
    public static Lens<T, TU> Compose<T, TInter, TU>(this Lens<T, TInter> lens1, Lens<TInter, TU> lens2) =>
        new (
            get: subject => lens2.Get(lens1.Get(subject)),
            set: (value, subject) => lens1.Set(lens2.Set(value, lens1.Get(subject)), subject)
        );

    /// <summary>
    /// Composes a <see cref="Lens{T,TU}"/> and a <see cref="Isomorphism{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the lens projection.</typeparam>
    /// <typeparam name="TU">The isomorphic type of the lens projection.</typeparam>
    /// <param name="lens">The lens to look through.</param>
    /// <param name="isomorphism">The isomorphism to apply.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<T, TU> Compose<T, TInter, TU>(this Lens<T, TInter> lens, Isomorphism<TInter, TU> isomorphism) =>
        new(
            get: subject => isomorphism.Get(lens.Get(subject)),
            set: (value, subject) => lens.Set(isomorphism.Get(value), subject)
        );

    /// <summary>
    /// Composes a <see cref="Lens{T,TU}"/> and a <see cref="Prism{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The type of the intermediate subject.</typeparam>
    /// <typeparam name="TU">The type of the output subject.</typeparam>
    /// <param name="lens">The <see cref="Lens{T,TU}"/> to look through first.</param>
    /// <param name="prism">The <see cref="Prism{T,TU}"/> to look through second.</param>
    /// <returns>A <see cref="Prism{T,TU}"/>.</returns>
    public static Prism<T, TU> Compose<T, TInter, TU>(this Lens<T, TInter> lens, Prism<TInter, TU> prism) =>
        new(
            get: subject => prism.Get(lens.Get(subject)),
            set: (value, subject) => lens.Set(prism.Set(value, lens.Get(subject)), subject));
}