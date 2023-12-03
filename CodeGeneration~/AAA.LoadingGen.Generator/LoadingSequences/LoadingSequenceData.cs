using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AAA.SourceGenerators.Common.CommonDiagnostics;

namespace AAA.LoadingGen.Generator;

public struct LoadingSequenceData : IEquatable<LoadingSequenceData>
{
    public string Name;
    public string? TargetNamespace;
    public Include Include;
    public ImmutableArray<string>? ExcludedSteps;
    public ImmutableArray<string>? ExcludedFeatures;
    public ImmutableArray<(string, string)>? SubstitutedSteps;
    public readonly List<string> AdditionalData = new();

    public LoadingSequenceData(string name, string targetNamespace)
    {
        Name = name;
        TargetNamespace = targetNamespace;
    }

    // public LoadingSequenceData(string name, string targetNamespace, LoadingStepsFilter loadingStepsFilter, ImmutableArray<string>? excludedSteps,
    //     ImmutableArray<string>? excludedFeatures,
    //     ImmutableArray<(string, string)>? substitutedSteps, List<string> additionalData)
    // {
    //     Name = name;
    //     TargetNamespace = targetNamespace;
    //     LoadingStepsFilter = loadingStepsFilter;
    //     ExcludedSteps = excludedSteps;
    //     ExcludedFeatures = excludedFeatures;
    //     SubstitutedSteps = substitutedSteps;
    //     AdditionalData = additionalData;
    // }

    public bool Equals(LoadingSequenceData other)
    {
        return Name == other.Name && TargetNamespace == other.TargetNamespace && Include == other.Include &&
               Nullable.Equals(ExcludedSteps, other.ExcludedSteps) && Nullable.Equals(ExcludedFeatures, other.ExcludedFeatures) &&
               Nullable.Equals(SubstitutedSteps, other.SubstitutedSteps);
    }

    public override bool Equals(object? obj)
    {
        return obj is LoadingSequenceData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ (TargetNamespace != null ? TargetNamespace.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)Include;
            hashCode = (hashCode * 397) ^ ExcludedSteps.GetHashCode();
            hashCode = (hashCode * 397) ^ ExcludedFeatures.GetHashCode();
            hashCode = (hashCode * 397) ^ SubstitutedSteps.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString()
    {
        return $@"LoadingSteps Include: {Include}
{ExcludedSteps?.Aggregate("\nExcluded Steps:", (s, s1) => $"{s}\n    {s1}")}
{ExcludedFeatures?.Aggregate("\nExcluded Features:", (s, s1) => $"{s}\n    {s1}")}
{SubstitutedSteps?.Aggregate("\nSubstituted Steps:", (s, s1) => $"{s}\n    ({s1.Item1}, {s1.Item2})")}
{(AdditionalData.Count <= 0 ? null : AdditionalData.Aggregate("\nAdditional Data:", (s, s1) => $"{s}\n    {s1}"))}";
    }

    public bool TryResolveAttribute(ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData, out Diagnostic? diagnostic)
    {
        diagnostic = null;
        return attributeData switch
        {
            { AttributeClass.Name: "LoadingSequenceAttribute" } => TryResolveLoadingSequenceAttribute(classDeclarationSyntax, attributeData, ref diagnostic),
            { AttributeClass.Name: "ExcludeLoadingStepAttribute" } => TryResolveExcludedStepsAttribute(attributeData),
            { AttributeClass.Name: "ExcludeLoadingFeatureAttribute" } => TryResolveExcludedFeaturesAttribute(attributeData),
            { AttributeClass.Name: "SubstituteLoadingStepAttribute" } => TryResolveSubstitutedStepsAttribute(classDeclarationSyntax, attributeData, ref diagnostic),
            _ => true
        };
    }

    private bool TryResolveLoadingSequenceAttribute(ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData, ref Diagnostic? diagnostic)
    {
        try
        {
            var targetLoadingStepsFilter = attributeData.ConstructorArguments.FirstOrDefault().Value;

            if (targetLoadingStepsFilter == null)
            {
                diagnostic = Diagnostic.Create(IncorrectAttributeData, classDeclarationSyntax.GetLocation(), "LoadingSequenceAttribute", classDeclarationSyntax.Identifier.Text);
                return false;
            }

            Include = (Include)targetLoadingStepsFilter;
            return true;
        }
        catch (Exception e)
        {
            AdditionalData.Add(e.ToString());
            return false;
        }
    }

    private bool TryResolveExcludedStepsAttribute(AttributeData attributeData)
    {
        ExcludedSteps = attributeData.ConstructorArguments
            .FirstOrDefault()
            .Values
            .Where(x => x.Value is not null)
            .Select(x => ((INamedTypeSymbol)x.Value!).Name)
            .ToImmutableArray();
        return true;
    }

    private bool TryResolveExcludedFeaturesAttribute(AttributeData attributeData)
    {
        ExcludedFeatures = attributeData.ConstructorArguments.FirstOrDefault().Values
            .Where(x => x.Value is not null)
            .Select(x => (string)x.Value!)
            .ToImmutableArray();
        return true;
    }

    private bool TryResolveSubstitutedStepsAttribute(ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData, ref Diagnostic? diagnostic)
    {
        var targetTypesArguments = attributeData.ConstructorArguments.FirstOrDefault().Values;
        var replacementTypesArguments = attributeData.ConstructorArguments.LastOrDefault().Values;

        if (targetTypesArguments.Any(x => x.Value is null) || replacementTypesArguments.Any(x => x.Value is null))
        {
            diagnostic = Diagnostic.Create(IncorrectAttributeData, classDeclarationSyntax.GetLocation(), "SubstituteLoadingStepAttribute", classDeclarationSyntax.Identifier.Text);
            return false;
        }

        //TODO: prevent boxing
        SubstitutedSteps = targetTypesArguments.Zip(replacementTypesArguments, (targetType, replacementType)
                => (((INamedTypeSymbol)targetType.Value!).Name, ((INamedTypeSymbol)replacementType.Value!).Name))
            .ToImmutableArray();
        return true;
    }
}