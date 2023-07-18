using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AAA.LoadingGen.Generator;

public struct LoadingStepData : IEquatable<LoadingStepData>
{
    public string Name;
    public string? TargetNamespace;
    public LoadingType LoadingType;
    public bool ExcludedByDefault;
    public ImmutableArray<string>? FeatureTags;
    public ImmutableArray<string>? Dependencies;
    public List<string> AdditionalData = new();

    public LoadingStepData(string name, string? targetNamespace, LoadingType loadingType, ImmutableArray<string>? featureTags, ImmutableArray<string>? dependencies,
        List<string> additionalData,
        bool excludedByDefault)
    {
        Name = name;
        TargetNamespace = targetNamespace;
        LoadingType = loadingType;
        FeatureTags = featureTags;
        Dependencies = dependencies;
        AdditionalData = additionalData;
        ExcludedByDefault = excludedByDefault;
    }

    public LoadingStepData(string className, string? targetNamespace)
    {
        Name = className;
        TargetNamespace = targetNamespace;
    }

    public bool TryResolveAttribute(ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData, out Diagnostic? diagnostic)
    {
        diagnostic = null;
        return attributeData switch
        {
            { AttributeClass.Name: "LoadingStepAttribute" } => TryResolveLoadingStepAttribute(classDeclarationSyntax, attributeData, ref diagnostic),
            { AttributeClass.Name: "FeatureTagAttribute" } => TryResolveFeatureTagAttribute(attributeData),
            { AttributeClass.Name: "RequiresLoadingDependencyAttribute" } => TryResolveDependenciesAttribute(attributeData),
            { AttributeClass.Name: "ExcludedByDefaultAttribute" } => TryResolveExcludedAttribute(),
            _ => true
        };
    }

    private bool TryResolveLoadingStepAttribute(ClassDeclarationSyntax classDeclarationSyntax, AttributeData attributeData, ref Diagnostic? diagnostic)
    {
        var targetLoadingType = attributeData.ConstructorArguments.FirstOrDefault().Value;
        if (targetLoadingType == null)
        {
            diagnostic = Diagnostic.Create(LoadingGenDiagnostics.IncorrectAttributeData, Location.None, "LoadingStepAttribute", classDeclarationSyntax.Identifier.Text);
            return false;
        }

        LoadingType = (LoadingType)targetLoadingType;
        return true;
    }

    private bool TryResolveFeatureTagAttribute(AttributeData attributeData)
    {
        FeatureTags = attributeData.ConstructorArguments.FirstOrDefault().Values
            .Where(x => x.Value is not null)
            .Select(x => (string)x.Value!)
            .ToImmutableArray();
        return true;
    }

    private bool TryResolveDependenciesAttribute(AttributeData attributeData)
    {
        Dependencies = attributeData.ConstructorArguments.FirstOrDefault().Values
            .Where(x => x.Value is not null)
            .Select(x => ((INamedTypeSymbol)x.Value!).Name)
            .ToImmutableArray();
        return true;
    }

    private bool TryResolveExcludedAttribute()
    {
        ExcludedByDefault = true;
        return true;
    }

    public bool HasFeatureTag(string featureTag)
    {
        if (FeatureTags is null)
            return false;

        foreach (var assignedFeatureTag in FeatureTags.Value)
        {
            if (assignedFeatureTag == featureTag)
                return true;
        }

        return false;
    }

    public bool Equals(LoadingStepData other)
    {
        return Name == other.Name && TargetNamespace == other.TargetNamespace && LoadingType == other.LoadingType && ExcludedByDefault == other.ExcludedByDefault &&
               Nullable.Equals(FeatureTags, other.FeatureTags) && Nullable.Equals(Dependencies, other.Dependencies);
    }

    public override bool Equals(object? obj)
    {
        return obj is LoadingStepData other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name.GetHashCode();
            hashCode = (hashCode * 397) ^ (TargetNamespace != null ? TargetNamespace.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (int)LoadingType;
            hashCode = (hashCode * 397) ^ ExcludedByDefault.GetHashCode();
            hashCode = (hashCode * 397) ^ FeatureTags.GetHashCode();
            hashCode = (hashCode * 397) ^ Dependencies.GetHashCode();
            return hashCode;
        }
    }

    public override string ToString()
    {
        return @$"LoadingType: {LoadingType}
{FeatureTags?.Aggregate("\nFeatureTags:", (s, s1) => $"{s}\n    {s1}")}
{Dependencies?.Aggregate("\nDependencies:", (s, s1) => $"{s}\n    {s1}")}
{(AdditionalData.Count <= 0 ? null : AdditionalData.Aggregate("\nAdditional Data:", (s, s1) => $"{s}\n    {s1}"))}";
    }
}