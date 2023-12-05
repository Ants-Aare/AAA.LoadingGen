using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AAA.SourceGenerators.Common.CommonDiagnostics;

namespace AAA.LoadingGen.Generator;

public struct LoadingSequenceData : IEquatable<LoadingSequenceData>, IAttributeResolver, ITypeResolver
{
    public string Name = string.Empty;
    public string? TargetNamespace;
    public Include Include = Include.All;
    public ImmutableArray<string>? ExcludedSteps;
    public ImmutableArray<string>? IncludedSteps;
    public ImmutableArray<string>? ExcludedFeatures;
    public ImmutableArray<string>? IncludedFeatures;
    public ImmutableArray<(string, string)>? SubstitutedSteps;
    public List<string> AdditionalData = new();
    
    public LoadingSequenceData() { }

    public void ResolveType(INamedTypeSymbol namedTypeSymbol)
    {
        Name = namedTypeSymbol.Name;
        TargetNamespace = namedTypeSymbol.GetNameSpaceString();
    }

    public Diagnostic? TryResolveAttribute(AttributeData attributeData)
    {
        return attributeData switch
        {
            { AttributeClass.Name: "LoadingSequenceAttribute" } => TryResolveLoadingSequenceAttribute(attributeData),
            { AttributeClass.Name: "ExcludeLoadingStepAttribute" } => TryResolveExcludedStepsAttribute(attributeData),
            { AttributeClass.Name: "IncludeLoadingStepAttribute" } => TryResolveIncludedStepsAttribute(attributeData),
            { AttributeClass.Name: "ExcludeLoadingFeatureAttribute" } => TryResolveExcludedFeaturesAttribute(attributeData),
            { AttributeClass.Name: "IncludeLoadingFeatureAttribute" } => TryResolveIncludedFeaturesAttribute(attributeData),
            { AttributeClass.Name: "SubstituteLoadingStepAttribute" } => TryResolveSubstitutedStepsAttribute(attributeData),
            _ => null
        };
    }

    private Diagnostic? TryResolveLoadingSequenceAttribute(AttributeData attributeData)
    {
        try
        {
            var targetLoadingStepsFilter = attributeData.ConstructorArguments.FirstOrDefault().Value;

            if (targetLoadingStepsFilter == null)
            {
                return Diagnostic.Create(IncorrectAttributeData, attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(), attributeData.AttributeClass?.Name);
            }

            Include = (Include)targetLoadingStepsFilter;
            return null;
        }
        catch (Exception e)
        {
            AdditionalData.Add(e.ToString());
            return null;
        }
    }

    private Diagnostic? TryResolveExcludedStepsAttribute(AttributeData attributeData)
    {
        ExcludedSteps = attributeData.ConstructorArguments
            .FirstOrDefault()
            .Values
            .Where(x => x.Value is not null)
            .Select(x => ((INamedTypeSymbol)x.Value!).Name)
            .ToImmutableArray();
        return null;
    }
    private Diagnostic? TryResolveIncludedStepsAttribute(AttributeData attributeData)
    {
        IncludedSteps = attributeData.ConstructorArguments
            .FirstOrDefault()
            .Values
            .Where(x => x.Value is not null)
            .Select(x => ((INamedTypeSymbol)x.Value!).Name)
            .ToImmutableArray();
        return null;
    }

    private Diagnostic? TryResolveExcludedFeaturesAttribute(AttributeData attributeData)
    {
        ExcludedFeatures = attributeData.ConstructorArguments.FirstOrDefault().Values
            .Where(x => x.Value is not null)
            .Select(x => (string)x.Value!)
            .ToImmutableArray();
        return null;
    }
    private Diagnostic? TryResolveIncludedFeaturesAttribute(AttributeData attributeData)
    {
        IncludedFeatures = attributeData.ConstructorArguments.FirstOrDefault().Values
            .Where(x => x.Value is not null)
            .Select(x => (string)x.Value!)
            .ToImmutableArray();
        return null;
    }

    private Diagnostic? TryResolveSubstitutedStepsAttribute(AttributeData attributeData)
    {
        var targetTypesArguments = attributeData.ConstructorArguments.FirstOrDefault().Values;
        var replacementTypesArguments = attributeData.ConstructorArguments.LastOrDefault().Values;

        if (targetTypesArguments.Any(x => x.Value is null) || replacementTypesArguments.Any(x => x.Value is null))
        {
            return Diagnostic.Create(IncorrectAttributeData, attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(), attributeData.AttributeClass?.Name);
        }

        //TODO: prevent boxing
        SubstitutedSteps = targetTypesArguments.Zip(replacementTypesArguments, (targetType, replacementType)
                => (((INamedTypeSymbol)targetType.Value!).Name, ((INamedTypeSymbol)replacementType.Value!).Name))
            .ToImmutableArray();
        return null;
    }
    
    
    
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
}