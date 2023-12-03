using System;
using System.Linq;
using System.Threading;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.LoadingGen.Generator.LoadingSteps;

public class LoadingStepProvider
{
    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration
               && classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword)
               && classDeclaration.AttributeLists
                   .SelectMany(x => x.Attributes)
                   .Any(x => x is { Name: IdentifierNameSyntax { Identifier.Text: "LoadingStep" } });
    }

    public static ResultOrDiagnostics<LoadingStepData> Transform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        try
        {
            if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol namedTypeSymbol)
                return Diagnostic.Create(CommonDiagnostics.NamedTypeSymbolNotFound, classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier);

            var attributeDatas = namedTypeSymbol.GetAttributes();

            var className = classDeclarationSyntax.Identifier.Text;
            var targetNamespace = namedTypeSymbol.ContainingNamespace.IsGlobalNamespace
                ? null
                : namedTypeSymbol.ContainingNamespace.ToDisplayString();

            var loadingStepData = new LoadingStepData(className, targetNamespace);

            foreach (var attributeData in attributeDatas)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (loadingStepData.TryResolveAttribute(classDeclarationSyntax, attributeData, out var diagnostic) && diagnostic != null)
                    return diagnostic;
            }

            return loadingStepData;
        }
        catch (Exception e)
        {
            return Diagnostic.Create(CommonDiagnostics.ExceptionOccured, classDeclarationSyntax.GetLocation(), nameof(Transform) + classDeclarationSyntax.Identifier.Text, e.ToString());
        }
    }
}