﻿namespace Refract;

/// <summary>
/// Provides an abstract view over a <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The source type to view.</typeparam>
/// <typeparam name="TU">The target type to view the subject as.</typeparam>
public readonly struct Isomorphism<T, TU> {
    readonly Func<T, TU> _forward;
    readonly Func<TU, T> _backward;

    /// <summary>
    /// Initializes a new <see cref="Isomorphism{T,TU}"/> by checking for cast operators on <typeparamref name="T"/>.
    /// </summary>
    public Isomorphism() {
        var (t, tu) = (typeof(T), typeof(TU));

        var forward = (
            from method in t.GetMethods()
            let parameters = method.GetParameters()
            where (method.Name == "op_Explicit" || method.Name == "op_Implicit") &&
                  parameters is [{ ParameterType: var parameterType }] && parameterType == t &&
                  method.ReturnType == tu
            select method).FirstOrDefault();

        var backward = (
            from method in t.GetMethods()
            let parameters = method.GetParameters()
            where (method.Name == "op_Explicit" || method.Name == "op_Implicit") &&
                  parameters is [{ ParameterType: var parameterType }] && parameterType == tu &&
                  method.ReturnType == t
            select method).FirstOrDefault();

        if (forward is null || backward is null) throw new InvalidOperationException("The given type cannot be converted to an Isomorphism because it doesn't define the appropriate cast operators.");
        _forward = sub => (TU)forward.Invoke(sub, [sub])!;
        _backward = sub => (T)backward.Invoke(sub, [sub])!;
    }

    /// <summary>
    /// Initializes a new <see cref="Isomorphism{T,TU}"/>.
    /// </summary>
    /// <param name="forward">A function to read a <typeparamref name="T"/> as a <typeparamref name="TU"/>.</param>
    /// <param name="backward">A function to read a <typeparamref name="TU"/> as a <typeparamref name="T"/>.</param>
    public Isomorphism(Func<T, TU> forward, Func<TU, T> backward) {
        _forward = forward;
        _backward = backward;
    }

    /// <summary>
    /// Reads the given <paramref name="subject"/> as a <typeparamref name="TU"/>.
    /// </summary>
    /// <param name="subject">The subject to read.</param>
    /// <returns>A <typeparamref name="TU"/>.</returns>
    public TU Get(T subject) => _forward(subject);

    /// <summary>
    /// Reads the given <paramref name="subject"/> as a <typeparamref name="TU"/>.
    /// </summary>
    /// <param name="subject">The subject to read.</param>
    /// <returns>A <typeparamref name="T"/>.</returns>
    public T Get(TU subject) => _backward(subject);

    /// <summary>
    /// Deconstructs a <see cref="Isomorphism{T,TU}"/> to its forward and backward functions respectively.
    /// </summary>
    /// <param name="forward">The forward function to deconstruct to.</param>
    /// <param name="backward">The backward function to deconstruct to.</param>
    public void Deconstruct(out Func<T, TU> forward, out Func<TU, T> backward) => (forward, backward) = (_forward, _backward);

    /// <summary>
    /// Implicitly casts the given <see cref="Isomorphism{T,TU}"/> to a <see cref="Func{TResult}"/> that reads a <typeparamref name="T"/>.
    /// </summary>
    /// <param name="instance">The <see cref="Isomorphism{T,TU}"/> to cast.</param>
    public static implicit operator Func<T, TU>(Isomorphism<T, TU> instance) => instance._forward;

    /// <summary>
    /// Implicitly casts the given <see cref="Isomorphism{T,TU}"/> to a <see cref="Action{T}"/> that reads to a <typeparamref name="TU"/>.
    /// </summary>
    /// <param name="instance">The <see cref="Isomorphism{T,TU}"/> to cast.</param>
    public static implicit operator Func<TU, T>(Isomorphism<T, TU> instance) => instance._backward;

    /// <summary>
    /// Creates a <see cref="Isomorphism{T,TU}"/> that is the inverse of the given <see cref="Isomorphism{T,TU}"/>.
    /// </summary>
    /// <param name="instance">The <see cref="Isomorphism{T,TU}"/> to inverse.</param>
    /// <returns>The inverse <see cref="Isomorphism{T,TU}"/>.</returns>
    public static Isomorphism<TU, T> operator !(Isomorphism<T, TU> instance) => new(instance._backward, instance._forward);
}

public static class Isomorphism {
    /// <summary>
    /// Creates a new <see cref="Isomorphism{T,TU}"/> for the given types.
    /// </summary>
    /// <remarks>The source type <typeparamref name="T"/> MUST implement cast operators to and from <typeparamref name="TU"/> for this method to work.</remarks>
    /// <typeparam name="T">The type of the source object.</typeparam>
    /// <typeparam name="TU">The type of the result object.</typeparam>
    /// <returns>A <see cref="Isomorphism{T,TU}"/>.</returns>
    public static Isomorphism<T, TU> Auto<T, TU>() => new();

    /// <summary>
    /// Creates a <see cref="Isomorphism{T,TU}"/> that is the inverse of the given <see cref="Isomorphism{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source object.</typeparam>
    /// <typeparam name="TU">The type of the result object.</typeparam>
    /// <param name="iso">The <see cref="Isomorphism{T,TU}"/> to inverse.</param>
    /// <returns>The inverse <see cref="Isomorphism{T,TU}"/>.</returns>
    public static Isomorphism<TU, T> Inverse<T, TU>(this Isomorphism<T, TU> iso) => !iso;

    /// <summary>
    /// Composes a <see cref="Isomorphism{T,TU}"/> and a <see cref="Lens{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The isomorphic type of the input subject.</typeparam>
    /// <typeparam name="TU">The type of the lens projection.</typeparam>
    /// <param name="isomorphism">The isomorphism to apply.</param>
    /// <param name="lens">The lens to look through.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<T, TU> Compose<T, TInter, TU>(this Isomorphism<T, TInter> isomorphism, Lens<TInter, TU> lens) =>
        new(
            get: subject => lens.Get(isomorphism.Get(subject)),
            set: (value, subject) => isomorphism.Get(lens.Set(value, isomorphism.Get(subject)))
        );

    /// <summary>
    /// Composes a <see cref="Isomorphism{T,TU}"/> and a <see cref="Lens{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The isomorphic type of the input subject.</typeparam>
    /// <typeparam name="TU">The type of the lens projection.</typeparam>
    /// <param name="isomorphism">The isomorphism to apply.</param>
    /// <param name="lens">The lens to look through.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<T, TU> Compose<T, TInter, TU>(this Isomorphism<TInter, T> isomorphism, Lens<TInter, TU> lens) =>
        new(
            get: subject => lens.Get(isomorphism.Get(subject)),
            set: (value, subject) => isomorphism.Get(lens.Set(value, isomorphism.Get(subject)))
        );

    /// <summary>
    /// Composes two <see cref="Isomorphism{T,TU}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the input subject.</typeparam>
    /// <typeparam name="TInter">The isomorphic type of the input subject.</typeparam>
    /// <typeparam name="TU">The isomorphic type twice removed from the input subject.</typeparam>
    /// <param name="iso1">The first isomorphism to apply.</param>
    /// <param name="iso2">The second isomorphism to apply.</param>
    /// <returns>A <see cref="Isomorphism{T,TU}"/>.</returns>
    public static Isomorphism<T, TU> Compose<T, TInter, TU>(this Isomorphism<T, TInter> iso1, Isomorphism<TInter, TU> iso2) =>
        new(
            forward: subject => iso2.Get(iso1.Get(subject)),
            backward: subject => iso1.Get(iso2.Get(subject))
        );
}