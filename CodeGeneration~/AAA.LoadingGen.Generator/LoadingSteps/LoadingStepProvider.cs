using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.LoadingGen.Generator.LoadingSteps;

public class LoadingStepProvider
{
    const string LoadingStepName = "LoadingStep";
    const string LoadingStepAttributeName = "LoadingStepAttribute";

    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration
           && classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword)
           && classDeclaration.AttributeLists
               .SelectMany(x => x.Attributes)
               .Any(x => x is { Name: IdentifierNameSyntax { Identifier.Text: LoadingStepName } });

    public static bool Filter(INamedTypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol.IsGenericType || namedTypeSymbol.IsAbstract || namedTypeSymbol.IsAnonymousType)
            return false;
        if (namedTypeSymbol.DeclaredAccessibility != Accessibility.Public || namedTypeSymbol.SpecialType != SpecialType.None)
            return false;
        
        var attributeDatas = namedTypeSymbol.GetAttributes();
        if (attributeDatas.Length == 0)
            return false;

        return attributeDatas.Any(x => x is { AttributeClass.Name: LoadingStepAttributeName });
    }
}