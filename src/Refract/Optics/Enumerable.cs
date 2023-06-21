using EnumerableEx = System.Linq.Enumerable;

namespace Refract.Optics; 

/// <summary>
/// Provides a number of optics for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class Enumerable {
    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that enumerates an <see cref="IEnumerable{T}"/> as an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/> that enumerates a <see cref="IEnumerable{T}"/> as an array.</returns>
    public static Lens<IEnumerable<T>, T[]> ToArray<T>() => new(EnumerableEx.ToArray);

    /// <summary>
    /// Creates a <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <returns>A <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/> or the default value if the sequence is empty..</returns>
    public static Prism<IEnumerable<T>, T> FirstOrDefault<T>() => new(EnumerableEx.FirstOrDefault);

    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>. Throws if the array is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>.</returns>
    public static Lens<IEnumerable<T>, T> First<T>() => new(EnumerableEx.First);

    /// <summary>
    /// Creates a <see cref="Lens"/> that returns the first element by composing <see cref="First{T}"/> onto a lens that returns an <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the source element of the first lens in the composition.</typeparam>
    /// <typeparam name="TU">The type of the elements in the array.</typeparam>
    /// <param name="lens">The <see cref="Lens{T,TU}"/> that produces the <see cref="IEnumerable{T}"/>.</param>
    /// <returns>A <see cref="Lens{T,TU}"/> that produces the first element in an <see cref="IEnumerable{T}"/>.</returns>
    public static Lens<T, TU> First<T, TU>(this Lens<T, IEnumerable<TU>> lens) => lens.Compose(First<TU>());

    /// <summary>
    /// Creates a <see cref="Lens"/> that maps the elements of the given sequence to another type.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TU">The type of the elements in the resulting sequence.</typeparam>
    /// <param name="selectorLens">The lens that maps a <typeparamref name="T"/> to a <typeparamref name="TU"/>.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<IEnumerable<T>, IEnumerable<TU>> Select<T, TU>(Lens<T, TU> selectorLens) => new(enumerable => enumerable.Select(selectorLens.Get));
}