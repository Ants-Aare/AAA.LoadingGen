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
        (LoadingSequenceData loadingSequenceData, ImmutableArray<LoadingStepData> stepDatas) data,
        CancellationToken cancellationToken)
    {
        // if (data.loadingSequenceData is null)
            // return new ResultOrDiagnostics<LoadingSequenceDataWithDependencies>();

        var filteredLoadingStepDatas = data.loadingSequenceData.Include switch
        {
            Include.None => new HashSet<LoadingStepData>(),
            Include.All => new HashSet<LoadingStepData>(data.stepDatas.Where(x => !x.ExcludedByDefault)),
            _ => throw new ArgumentOutOfRangeException()
        };


        if (data.loadingSequenceData.IncludedFeatures is not null)
        {
            foreach (var includedFeatureTag in data.loadingSequenceData.IncludedFeatures.Value)
            {
                data.loadingSequenceData.AdditionalData.Add($"Added Feature {includedFeatureTag}:");

                foreach (var stepData in data.stepDatas)
                {
                    if (stepData.HasFeatureTag(includedFeatureTag))
                    {
                        data.loadingSequenceData.AdditionalData.Add($"    Added Step {stepData.Name}.");
                        filteredLoadingStepDatas.Add(stepData);
                    }
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
        }


        if (data.loadingSequenceData.ExcludedFeatures is not null)
        {
            foreach (var excludedFeatureTag in data.loadingSequenceData.ExcludedFeatures.Value)
            {
                data.loadingSequenceData.AdditionalData.Add($"Removed Feature {excludedFeatureTag}:");
                filteredLoadingStepDatas.RemoveWhere(x =>
                {
                    var hasFeatureTag = x.HasFeatureTag(excludedFeatureTag);
                    if (hasFeatureTag)
                        data.loadingSequenceData.AdditionalData.Add($"    Removed Step {x.Name}.");
                    return hasFeatureTag;
                });
            }

            cancellationToken.ThrowIfCancellationRequested();
        }


        if (data.loadingSequenceData.ExcludedSteps is not null)
        {
            foreach (var excludedStep in data.loadingSequenceData.ExcludedSteps.Value)
            {
                data.loadingSequenceData.AdditionalData.Add($"Removed excluded step: {excludedStep}");
                filteredLoadingStepDatas.RemoveWhere(x => x.Name == excludedStep);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (data.loadingSequenceData.IncludedSteps is not null)
        {
            foreach (var includedStep in data.loadingSequenceData.IncludedSteps.Value)
            {
                var step = data.stepDatas.FirstOrDefault(x => x.Name == includedStep);
                if (step is null)
                    continue;
                data.loadingSequenceData.AdditionalData.Add($"Added included step {includedStep}.");
                filteredLoadingStepDatas.Add(step);
            }

            cancellationToken.ThrowIfCancellationRequested();
        }


        if (data.loadingSequenceData.SubstitutedSteps is not null)
        {
            foreach ((string target, string replacement) substitutedStep in data.loadingSequenceData.SubstitutedSteps)
            {
                var replacementStep = data.stepDatas.FirstOrDefault(x => x.Name == substitutedStep.replacement);
                if (replacementStep is null)
                    continue;
                filteredLoadingStepDatas.RemoveWhere(x => x.Name == substitutedStep.target);
                filteredLoadingStepDatas.Add(replacementStep);
                data.loadingSequenceData.AdditionalData.Add($"Substituted step {substitutedStep.target} with {substitutedStep.replacement}.");

                //TODO: Update dependencies here
            }
        }

        return new LoadingSequenceDataWithDependencies(data.loadingSequenceData, filteredLoadingStepDatas.ToImmutableArray());
    }
}