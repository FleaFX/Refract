using System.Diagnostics.CodeAnalysis;

namespace Refract.Optics; 

public static class Primitives {
    /// <summary>
    /// Creates a <see cref="Isomorphism{T,TU}"/> that views a <see cref="string"/> as a <see cref="Guid"/> and vice versa.
    /// </summary>
    /// <param name="format">The format of the <see cref="Guid"/>.</param>
    /// <returns>A <see cref="Isomorphism{T,TU}"/>.</returns>
    public static Isomorphism<string, Guid> StringAsGuid([StringSyntax(StringSyntaxAttribute.GuidFormat)] string? format = "D") =>
        new(str => new Guid(str), guid => guid.ToString(format));
}