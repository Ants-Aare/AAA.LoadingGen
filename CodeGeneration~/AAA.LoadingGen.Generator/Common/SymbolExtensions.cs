using Microsoft.CodeAnalysis;

namespace AAA.SourceGenerators.Common;

public static class SymbolExtensions
{
    public static string? GetNameSpaceString(this INamedTypeSymbol namedTypeSymbol)
        => namedTypeSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : namedTypeSymbol.ContainingNamespace.ToDisplayString();

}