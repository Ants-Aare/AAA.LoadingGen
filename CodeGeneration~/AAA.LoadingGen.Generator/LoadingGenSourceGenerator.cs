using System;
using System.Collections.Immutable;
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
        var optionsProvider = context.AnalyzerConfigOptionsProvider;

        var loadingSteps = context.SyntaxProvider
            .CreateSyntaxProvider(LoadingStepProvider.Filter, CommonTransforms.TransformResolved<LoadingStepData>)
            .HandleDiagnostics(context);

        // var loadingSequences = context.SyntaxProvider
        //     .CreateSyntaxProvider(LoadingSequenceProvider.Filter, CommonTransforms.TransformResolved<LoadingSequenceData>)
        //     .HandleDiagnostics(context);

        // var loadingStepsInCompilation =
        //     context.CompilationProvider
        //         .Select(static (c, ct) => CommonTransforms.TransformResolved<LoadingStepData>(c, ct, LoadingStepProvider.Filter))
        //         .HandleDiagnostics(context);

        // var loadingSequencesWithDependencies = loadingSequences
        //     .Combine(loadingSteps.Collect())
        //     .Select(LoadingSequenceProvider.FilterDependencies)
        //     .HandleDiagnostics(context);

        context.RegisterSourceOutput(loadingSteps, LoadingStepGenerator.GenerateOutput);
        // context.RegisterSourceOutput(loadingSequencesWithDependencies, LoadingSequenceGenerator.GenerateOutput);
    }
}