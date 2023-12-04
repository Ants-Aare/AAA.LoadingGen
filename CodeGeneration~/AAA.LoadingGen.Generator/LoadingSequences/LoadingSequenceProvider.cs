using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.LoadingGen.Generator.LoadingSequences;

public class LoadingSequenceProvider
{
    public static bool Filter(SyntaxNode node, CancellationToken cancellationToken)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 } classDeclaration
               && classDeclaration.Modifiers.Any(SyntaxKind.PublicKeyword)
               && classDeclaration.AttributeLists
                   .SelectMany(x => x.Attributes)
                   .Any(x => x is { Name: IdentifierNameSyntax { Identifier.Text: "LoadingSequence" } });
    }

    public static ResultOrDiagnostics<LoadingSequenceDataWithDependencies> FilterDependencies(
        (LoadingSequenceData loadingSequenceData, ImmutableArray<LoadingStepData> loadingStepDatas) data,
        CancellationToken cancellationToken)
    {
        var filteredLoadingStepDatas = data.loadingSequenceData.Include switch
        {
            Include.None => new HashSet<LoadingStepData>(),
            Include.All => new HashSet<LoadingStepData>(data.loadingStepDatas.Where(x => !x.ExcludedByDefault)),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (data.loadingSequenceData.ExcludedSteps is not null)
        {
            foreach (var excludedStep in data.loadingSequenceData.ExcludedSteps.Value)
            {
                data.loadingSequenceData.AdditionalData.Add($"Removed excluded step: {excludedStep}");
                filteredLoadingStepDatas.RemoveWhere(x => x.Name == excludedStep);
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (data.loadingSequenceData.ExcludedFeatures is not null)
        {
            foreach (var excludedFeatureTag in data.loadingSequenceData.ExcludedFeatures.Value)
            {
                filteredLoadingStepDatas.RemoveWhere(x => x.HasFeatureTag(excludedFeatureTag));
            }
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (data.loadingSequenceData.SubstitutedSteps is not null)
        {
            foreach ((string target, string replacement) substitutedStep in data.loadingSequenceData.SubstitutedSteps)
            {
                filteredLoadingStepDatas.RemoveWhere(x => x.Name == substitutedStep.target);
                var replacementStep = data.loadingStepDatas.FirstOrDefault(x => x.Name == substitutedStep.replacement);
                filteredLoadingStepDatas.Add(replacementStep);
            }
        }

        return new LoadingSequenceDataWithDependencies(data.loadingSequenceData, filteredLoadingStepDatas.ToImmutableArray());
    }
}