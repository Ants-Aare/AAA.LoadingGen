using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.LoadingGen.Generator;

[Generator]
public class LoadingStepsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // var loadingSequenceOrDiagnostic = context.SyntaxProvider
        //     .CreateSyntaxProvider(LoadingSequenceFilter, LoadingSequenceTransform);

        var loadingStepOrDiagnostic = context.SyntaxProvider
            .CreateSyntaxProvider(LoadingStepFilter, LoadingStepTransform);

        var diagnostics = loadingStepOrDiagnostic
            .Where(x => x.Diagnostic is not null)
            .Select((s, _) => s.Diagnostic!);

        var loadingStepDeclarations = loadingStepOrDiagnostic
            // .Where(x => x.Result is not null)
            .Select((s, _) => s.Result);

        context.RegisterSourceOutput(diagnostics, LoadingGenDiagnostics.ReportDiagnostics);
        context.RegisterSourceOutput(loadingStepDeclarations, GenerateLoadingStepOutput);
        context.RegisterPostInitializationOutput(x=> x.AddSource("Test.Generated.cs", "public partial class CreateGameManagerLoadingStep : ILoadingStep{}"));
    }

    private void GenerateLoadingStepOutput(SourceProductionContext sourceProductionContext, LoadingStepData loadingStepData)
    {
        var output =
            @$"namespace {loadingStepData.TargetNamespace}
{{
    public partial class {loadingStepData.Name} : ILoadingStep
    {{
    }}
}}";
        sourceProductionContext.AddSource($"{loadingStepData.Name}.Generated.cs", output);
    }

    static bool LoadingStepFilter(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration
               && classDeclaration.AttributeLists
                   .SelectMany(x => x.Attributes)
                   .Any(x => x is { Name: IdentifierNameSyntax { Identifier.Text: "LoadingStep" } })
               && !classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword);
    }

    static bool LoadingSequenceFilter(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration
               && classDeclaration.AttributeLists
                   .SelectMany(x => x.Attributes)
                   .Any(x => x is { Name: IdentifierNameSyntax { Identifier.Text: "LoadingSequence" } })
               && !classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword);
    }

    static ResultOrDiagnostics<LoadingStepData> LoadingStepTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = ModelExtensions.GetDeclaredSymbol(semanticModel: context.SemanticModel, declaration: classDeclarationSyntax, cancellationToken: cancellationToken);

        if (symbol is null)
            return Diagnostic.Create(descriptor: LoadingGenDiagnostics.NamedTypeSymbolNotFound, location: Location.None, messageArgs: classDeclarationSyntax.Identifier);

        var attributeDatas = symbol.GetAttributes();

        string loadingType = "";
        var className = classDeclarationSyntax.Identifier.Text;
        var targetNamespace = symbol.ContainingNamespace.ToDisplayString();
        string[]? featureTags = null;
        string[]? dependencies = null;

        foreach (var attributeData in attributeDatas)
        {
            switch (attributeData)
            {
                case { AttributeClass.Name: "LoadingStep" }:
                    loadingType = attributeData.ConstructorArguments.FirstOrDefault().ToString();
                    break;

                case { AttributeClass.Name: "FeatureTag" }:

                    var constructorArguments = attributeData.ConstructorArguments.FirstOrDefault().Values;
                    featureTags = new string[constructorArguments.Length];

                    for (var i = 0; i < constructorArguments.Length; i++)
                    {
                        featureTags[i] = constructorArguments[i].ToString() + constructorArguments[i].Value;
                    }

                    break;

                case { AttributeClass.Name: "RequiresLoadingDependency" }:
                    var arguments = attributeData.ConstructorArguments.FirstOrDefault().Values;
                    dependencies = new string[constructorArguments.Length];

                    for (var i = 0; i < constructorArguments.Length; i++)
                    {
                        dependencies[i] = constructorArguments[i].ToString() + constructorArguments[i].Value;
                    }

                    break;
            }
        }


        return new LoadingStepData(name: className, targetNamespace, loadingType, featureTags, dependencies: dependencies);
    }

    static ResultOrDiagnostics<LoadingSequenceData> LoadingSequenceTransform(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        var symbol = ModelExtensions.GetDeclaredSymbol(semanticModel: context.SemanticModel, declaration: classDeclarationSyntax, cancellationToken: cancellationToken);

        if (symbol is null)
            return Diagnostic.Create(descriptor: LoadingGenDiagnostics.NamedTypeSymbolNotFound, location: Location.None, messageArgs: classDeclarationSyntax.Identifier);

        var attributeDatas = symbol.GetAttributes();

        // string loadingType = "";
        var className = classDeclarationSyntax.Identifier.Text;
        var targetNamespace = symbol.ContainingNamespace.ToDisplayString();
        // string[]? featureTags = null;
        // string[]? dependencies = null;

        // foreach (var attributeData in attributeDatas)
        // {
        //     switch (attributeData)
        //     {
        //         case { AttributeClass.Name: "LoadingStep" }:
        //             loadingType = attributeData.ConstructorArguments.FirstOrDefault().ToString();
        //             break;
        //
        //         case { AttributeClass.Name: "FeatureTag" }:
        //
        //             var constructorArguments = attributeData.ConstructorArguments.FirstOrDefault().Values;
        //             featureTags = new string[constructorArguments.Length];
        //
        //             for (var i = 0; i < constructorArguments.Length; i++)
        //             {
        //                 featureTags[i] = constructorArguments[i].ToString() + constructorArguments[i].Value;
        //             }
        //
        //             break;
        //
        //         case { AttributeClass.Name: "RequiresLoadingDependency" }:
        //             var arguments = attributeData.ConstructorArguments.FirstOrDefault().Values;
        //             dependencies = new string[constructorArguments.Length];
        //
        //             for (var i = 0; i < constructorArguments.Length; i++)
        //             {
        //                 dependencies[i] = constructorArguments[i].ToString() + constructorArguments[i].Value;
        //             }
        //
        //             break;
        //     }
        // }


        return new LoadingSequenceData(className, targetNamespace); //new LoadingStepData(name: className, targetNamespace, loadingType, featureTags, dependencies: dependencies);
    }
}