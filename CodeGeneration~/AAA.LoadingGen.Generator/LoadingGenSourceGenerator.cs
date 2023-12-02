using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using AAA.LoadingGen.Generator.LoadingSequences;
using AAA.LoadingGen.Generator.LoadingSteps;
using AAA.LoadingGen.LoadingSequences;
using AAA.LoadingGen.LoadingSteps;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.LoadingGen.Generator;

[Generator]
public class LoadingGenSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var loadingSteps = context.SyntaxProvider
            .CreateSyntaxProvider(LoadingStepProvider.Filter, LoadingStepProvider.Transform)
            .HandleDiagnostics(context);

        var loadingSequences = context.SyntaxProvider
            .CreateSyntaxProvider(LoadingSequenceProvider.Filter, LoadingSequenceProvider.Transform)
            .HandleDiagnostics(context);
        
        var loadingSequencesWithDependencies = loadingSequences
            .Combine(loadingSteps.Collect())
            .Select(LoadingSequenceProvider.FilterDependencies)
            .HandleDiagnostics(context);
        // .Select((x, _) => new LoadingSequenceDataWithDependencies(x, new ImmutableArray<LoadingStepData>()));


//         var texts = context.AdditionalTextsProvider;
//         var paths = texts.Select((x, _)=> x.Path);
//         var collect = paths.Collect();
//         context.RegisterSourceOutput(collect, (ctx, c) =>
//         {
//             var stringBuilder = new StringBuilder(@"using UnityEngine;
// //[CreateAssetMenu(menuName = ""LoadingGen/AdditionalTexts"")]
//     public partial class AdditionalTexts //: ScriptableObject
// {public int number;}");
//             foreach (var content in c)
//             {
//                 stringBuilder.AppendLine($"//{c}");
//             }
//             ctx.AddSource("AdditionalTexts.cs", stringBuilder.ToString());
//         });
        context.RegisterSourceOutput(loadingSteps, LoadingStepGenerator.GenerateOutput);
        context.RegisterSourceOutput(loadingSequencesWithDependencies, LoadingSequenceGenerator.GenerateOutput);
    }

    private (string? name, string? source) Transform(GeneratorSyntaxContext syntaxContext, CancellationToken _)
    {
        try
        {
            var candidate = (ClassDeclarationSyntax)syntaxContext.Node;
            var select = candidate.BaseList?.Types.Select(x => x.Type.GetType() + " " + x.Type switch
                {
                    IdentifierNameSyntax identifierNameSyntax => identifierNameSyntax.Identifier.Text,
                    QualifiedNameSyntax qualifiedNameSyntax => $"{qualifiedNameSyntax.Left.GetType()} {qualifiedNameSyntax.Right.GetType()}",
                    _ => "throw new ArgumentOutOfRangeException()"
                })
                .ToArray();
            if (select == null) return (null, null);
            if (select.Length == 0) return (null, null);
            return (candidate.Identifier.Text, select.Aggregate($"public partial class {candidate.Identifier.Text}{{}}\n//BaseTypes: ", (s, text) => $"{s}\n//{text}"));
        }
        catch (Exception e)
        {
            return (e.Message, e.Message);
        }
    }
}