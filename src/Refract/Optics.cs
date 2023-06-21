namespace Refract;

/// <summary>
/// Contains general purpose lenses.
/// </summary>
public static class Optics {
    /// <summary>
    /// Creates a <see cref="Prism{T,TU}"/> that returns the first element in an array.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <typeparamref name="T"/>.</returns>
    public static Prism<T[], T> First<T>() => new(array => array.Length > 0 ? array[0] : default);

    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that filters an array whereby only elements that are not <see langword="null" /> satisfy the filter.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A filtered array of <typeparamref name="T"/>.</returns>
    public static Lens<T?[], T[]> HasValue<T>() => new(options => options.Where(val => val is not null).Select(val => val!).ToArray());
}