using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.SourceGenerators.Common;

public static class CommonTransforms
{
    public static ResultOrDiagnostics<T> TransformAttributeResolved<T>(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        where T : IAttributeResolver, ITypeResolver, new()
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        try
        {
            if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol namedTypeSymbol)
                return Diagnostic.Create(CommonDiagnostics.NamedTypeSymbolNotFound, classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text);
            
            var instance = new T();
            instance.ResolveType(classDeclarationSyntax.Identifier.Text, namedTypeSymbol.GetNameSpaceString());
            
            var attributeDatas = namedTypeSymbol.GetAttributes();
            foreach (var attributeData in attributeDatas)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!instance.TryResolveAttribute(classDeclarationSyntax, attributeData, out var diagnostic) && diagnostic != null)
                    return diagnostic;
            }

            return instance;
        }
        catch (Exception e)
        {
            return Diagnostic.Create(CommonDiagnostics.ExceptionOccured, classDeclarationSyntax.GetLocation(), nameof(TransformAttributeResolved) + classDeclarationSyntax.Identifier.Text, e.ToString());
        }
    }
}

public interface IAttributeResolver
{
    bool TryResolveAttribute(ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData, out Diagnostic? diagnostic);
}
public interface ITypeResolver
{
    public void ResolveType(string name, string? namespaceName);
}