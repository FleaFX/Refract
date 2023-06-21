namespace Refract;

/// <summary>
/// Contains general purpose lenses.
/// </summary>
public static class Optics {
    /// <summary>
    /// Creates a <see cref="Prism{T,TU}"/> that returns the first element in an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Prism{T,TU}"/>.</returns>
    public static Prism<T[], T> FirstOrDefault<T>() => new(array => array.Length > 0 ? array[0] : default);

    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that returns the first element in an array. Throws if the array is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<T[], T> First<T>() => new(array => array[0]);

    /// <summary>
    /// Creates a <see cref="Lens"/> that maps the elements of the given sequence to another type.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TU">The type of the elements in the resulting sequence.</typeparam>
    /// <param name="selectorFunc">The function that maps a <typeparamref name="T"/> to a <typeparamref name="TU"/>.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<IEnumerable<T>, IEnumerable<TU>> Select<T, TU>(Func<T, TU> selectorFunc) => new(enumerable => enumerable.Select(selectorFunc));

    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that filters an array whereby only elements that are not <see langword="null" /> satisfy the filter.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<T?[], T[]> HasValue<T>() => new(options => options.Where(val => val is not null).Select(val => val!).ToArray());
}