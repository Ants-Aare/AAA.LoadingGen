using System;
using System.Linq;
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
            instance.ResolveType(classDeclarationSyntax.Identifier.Text, namedTypeSymbol.GetNameSpaceString(), namedTypeSymbol);

            var attributeDatas = namedTypeSymbol.GetAttributes();
            foreach (var attributeData in attributeDatas)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var diagnostic = instance.TryResolveAttribute(classDeclarationSyntax, attributeData);
                if (diagnostic is not null)
                    return diagnostic;
            }

            return instance;
        }
        catch (Exception e)
        {
            return Diagnostic.Create(CommonDiagnostics.ExceptionOccured, classDeclarationSyntax.GetLocation(),
                nameof(TransformAttributeResolved) + classDeclarationSyntax.Identifier.Text, e.ToString());
        }
    }

    public static ResultOrDiagnostics<T> TransformAttributeCtorResolved<T>(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        where T : IAttributeResolver, ITypeResolver, IConstructorResolver, new()
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        try
        {
            if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol namedTypeSymbol)
                return Diagnostic.Create(CommonDiagnostics.NamedTypeSymbolNotFound, classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text);

            var instance = new T();
            instance.ResolveType(classDeclarationSyntax.Identifier.Text, namedTypeSymbol.GetNameSpaceString(), namedTypeSymbol);

            var attributeDatas = namedTypeSymbol.GetAttributes();
            foreach (var attributeData in attributeDatas)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var diagnostic = instance.TryResolveAttribute(classDeclarationSyntax, attributeData);
                if (diagnostic is not null)
                    return diagnostic;
            }

            foreach (var constructor in namedTypeSymbol.Constructors)
            {
                if (constructor is null)
                    continue;
                cancellationToken.ThrowIfCancellationRequested();

                var diagnostic = instance.TryResolveConstructor(constructor);
                if (diagnostic is not null)
                    return diagnostic;
            }

            return instance;
        }
        catch (Exception e)
        {
            return Diagnostic.Create(CommonDiagnostics.ExceptionOccured, classDeclarationSyntax.GetLocation(),
                nameof(TransformAttributeCtorResolved) + classDeclarationSyntax.Identifier.Text, e.ToString());
        }
    }
}

public interface IConstructorResolver
{
    public Diagnostic? TryResolveConstructor(IMethodSymbol ctor);
}

public interface IAttributeResolver
{
    Diagnostic? TryResolveAttribute(ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData);
}

public interface ITypeResolver
{
    public void ResolveType(string name, string? namespaceName, INamedTypeSymbol namedTypeSymbol);
}