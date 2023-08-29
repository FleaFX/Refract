using System.Collections.Immutable;

namespace Refract.Optics; 

/// <summary>
/// Provides a number of optics for <see cref="ImmutableDictionary{TKey,TValue}"/>
/// </summary>
public static class ImmutableDictionary {
    /// <summary>
    /// <see cref="Lens{T,TU}"/> that gets or sets a value in a <see cref="ImmutableDictionary{TKey,TValue}"/> for the given <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key elements.</typeparam>
    /// <typeparam name="TValue">The type of the value elements.</typeparam>
    /// <param name="key">The key to focus on.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<ImmutableDictionary<TKey, TValue>, TValue> Key<TKey, TValue>(TKey key) where TKey : notnull =>
        new(dict => dict[key], (value, dict) => dict.SetItem(key, value));


    /// <summary>
    /// <see cref="Lens{T,TU}"/> that gets or sets a value in a <see cref="ImmutableDictionary{TKey,TValue}"/> for the given <paramref name="key"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key elements.</typeparam>
    /// <typeparam name="TValue">The type of the value elements.</typeparam>
    /// <param name="key">The key to focus on.</param>
    /// <param name="defaultValue">The default value to return if no matching key was found.</param>
    /// <returns>A <see cref="Lens{T,TU}"/>.</returns>
    public static Lens<ImmutableDictionary<TKey, TValue>, TValue> KeyOrDefault<TKey, TValue>(TKey key, TValue defaultValue) where TKey : notnull =>
        new(dict => dict.GetValueOrDefault(key, defaultValue), (value, dict) => dict.SetItem(key, value));
}