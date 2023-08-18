namespace Refract.Optics;

/// <summary>
/// Provides a number of optics for <see cref="IAsyncEnumerable{T}"/>.
/// </summary>
public static class AsyncEnumerable {
    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that enumerates an <see cref="IEnumerable{T}"/> as an array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/> that enumerates a <see cref="IEnumerable{T}"/> as an array.</returns>
    public static Lens<IAsyncEnumerable<T>, T[]> ToArrayAsync<T>() => new(asyncEnumerable => asyncEnumerable.ToArrayAsync().GetAwaiter().GetResult());

    /// <summary>
    /// Creates a <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <returns>A <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/> or the default value if the sequence is empty..</returns>
    public static Prism<IAsyncEnumerable<T>, T> FirstOrDefaultAsync<T>() => new(asyncEnumerable => asyncEnumerable.FirstOrDefaultAsync().GetAwaiter().GetResult());

    /// <summary>
    /// Creates a <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="predicateLens">A <see cref="Lens{T,TU}"/> that implements the predicate to determine which element to return.</param>
    /// <returns>A <see cref="Prism{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/> or the default value if the sequence is empty..</returns>
    public static Prism<IAsyncEnumerable<T>, T> FirstOrDefaultAsync<T>(Lens<T, bool> predicateLens) => new(asyncEnumerable => asyncEnumerable.FirstOrDefaultAsync(predicateLens.Get).GetAwaiter().GetResult());

    /// <summary>
    /// Creates a <see cref="Lens{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>. Throws if the array is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/> that returns the first element in an <see cref="IEnumerable{T}"/>.</returns>
    public static Lens<IAsyncEnumerable<T>, T> First<T>() => new(asyncEnumerable => asyncEnumerable.FirstAsync().GetAwaiter().GetResult());

    /// <summary>
    /// Creates a <see cref="Lens"/> that returns the first element by composing <see cref="First{T}"/> onto a lens that returns an <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the source element of the first lens in the composition.</typeparam>
    /// <typeparam name="TU">The type of the elements in the array.</typeparam>
    /// <param name="lens">The <see cref="Lens{T,TU}"/> that produces the <see cref="IEnumerable{T}"/>.</param>
    /// <returns>A <see cref="Lens{T,TU}"/> that produces the first element in an <see cref="IEnumerable{T}"/>.</returns>
    public static Lens<T, TU> First<T, TU>(this Lens<T, IAsyncEnumerable<TU>> lens) => lens.Compose(First<TU>());

    /// <summary>
    /// Creates a <see cref="Lens"/> that maps the elements of the given sequence to another type.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
    /// <typeparam name="TU">The type of the elements in the resulting sequence.</typeparam>
    /// <param name="selectorLens">The lens that maps a <typeparamref name="T"/> to a <typeparamref name="TU"/>.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<IAsyncEnumerable<T>, IAsyncEnumerable<TU>> Select<T, TU>(Lens<T, TU> selectorLens) => new(asyncEnumerable => asyncEnumerable.Select(selectorLens.Get));

    /// <summary>
    /// Creates a <see cref="Lens"/> that filters the element of a given sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <param name="filterLens">The lens that implements a predicate to determine which elements satisfy the filter.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<IAsyncEnumerable<T>, IAsyncEnumerable<T>> Where<T>(Lens<T, bool> filterLens) => new(asyncEnumerable => asyncEnumerable.Where(filterLens.Get));

    /// <summary>
    /// Creates a <see cref="Lens"/> that groups the elements of a sequence according to a specified key selector lens.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the key element to group by.</typeparam>
    /// <param name="keySelectorLens">The lens that implements the key extraction function.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<IAsyncEnumerable<T>, IAsyncEnumerable<IAsyncGrouping<TKey, T>>> GroupBy<T, TKey>(Lens<T, TKey> keySelectorLens) => new(asyncEnumerable => asyncEnumerable.GroupBy(keySelectorLens.Get));

    /// <summary>
    /// Creates a <see cref="Lens"/> that returns a <see cref="IAsyncEnumerable{T}"/> that contains only distinct elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<IAsyncEnumerable<T>, IAsyncEnumerable<T>> Distinct<T>() => new(asyncEnumerable => asyncEnumerable.Distinct());

    /// <summary>
    /// Creates a <see cref="Lens"/> that returns a <see cref="IAsyncEnumerable{T}"/> that contains only distinct elements.
    /// </summary>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use when comparing the source elements.</param>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<IAsyncEnumerable<T>, IAsyncEnumerable<T>> Distinct<T>(IEqualityComparer<T> comparer) => new(asyncEnumerable => asyncEnumerable.Distinct(comparer));

    /// <summary>
    /// Gets the key of a <see cref="IAsyncGrouping{TKey,TElement}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    /// <typeparam name="TKey">The type of the grouping element.</typeparam>
    /// <returns>A <typeparamref name="TKey"/>.</returns>
    public static Lens<IAsyncGrouping<TKey, T>, TKey> Key<T, TKey>() => new(grouping => grouping.Key);
}
