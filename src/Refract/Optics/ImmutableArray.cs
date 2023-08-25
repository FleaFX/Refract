using System.Collections.Immutable;

namespace Refract.Optics; 

/// <summary>
/// Provides a number of optics for <see cref="ImmutableArray{T}"/>.
/// </summary>
public static class ImmutableArray {
    /// <summary>
    /// <see cref="Lens{T,TU}"/> that gets or sets a value in a <see cref="ImmutableArray{T}"/> at the given <paramref name="index"/>.
    /// </summary>
    /// <typeparam name="T">The type of the array elements.</typeparam>
    /// <param name="index">The index to focus on.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<ImmutableArray<T>, T> ElementAt<T>(int index) =>
        new(
            array => array[index],
            (value, array) => array.SetItem(index, value)
        );
}