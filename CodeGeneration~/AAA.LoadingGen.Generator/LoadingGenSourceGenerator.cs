using System;
using System.Linq;
using System.Threading;
using AAA.LoadingGen.Generator.LoadingSequences;
using AAA.LoadingGen.Generator.LoadingSteps;
using AAA.LoadingGen.LoadingSequences;
using AAA.LoadingGen.LoadingSteps;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.LoadingGen.Generator;

[Generator]
public class LoadingGenSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var loadingSteps = context.SyntaxProvider
            .CreateSyntaxProvider(LoadingStepProvider.Filter, CommonTransforms.TransformAttributeCtorResolved<LoadingStepData>)
            .HandleDiagnostics(context);

        // var loadingSequences = context.SyntaxProvider
        //     .CreateSyntaxProvider(LoadingSequenceProvider.Filter, CommonTransforms.TransformAttributeResolved<LoadingSequenceData>)
        //     .HandleDiagnostics(context);
        //
        // var loadingSequencesWithDependencies = loadingSequences
        //     .Combine(loadingSteps.Collect())
        //     .Select(LoadingSequenceProvider.FilterDependencies)
        //     .HandleDiagnostics(context);

        context.RegisterSourceOutput(loadingSteps, LoadingStepGenerator.GenerateOutput);
        // context.RegisterSourceOutput(loadingSequencesWithDependencies, LoadingSequenceGenerator.GenerateOutput);
    }
}