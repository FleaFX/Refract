namespace Refract.Optics; 

public static class Array {

    /// <summary>
    /// Creates a <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <returns>A <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/> or the default value if the sequence is empty..</returns>
    public static Prism<T[], T> FirstOrDefault<T>() => new(arr => arr.Length > 0 ? arr[0] : default);

    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>. Throws if the array is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>.</returns>
    public static Lens<T[], T> First<T>() => new(arr => arr[0]);

    /// <summary>
    /// Creates a <see cref="Lens"/> that returns the first element by composing <see cref="First{T}"/> onto a lens that returns an <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the source element of the first lens in the composition.</typeparam>
    /// <typeparam name="TU">The type of the elements in the array.</typeparam>
    /// <param name="lens">The <see cref="Lens{T,TU}"/> that produces the <see cref="IEnumerable{T}"/>.</param>
    /// <returns>A <see cref="Lens{T,TU}"/> that produces the first element in an <see cref="IEnumerable{T}"/>.</returns>
    public static Lens<T, TU> First<T, TU>(this Lens<T, TU[]> lens) => lens.Compose(First<TU>());
}