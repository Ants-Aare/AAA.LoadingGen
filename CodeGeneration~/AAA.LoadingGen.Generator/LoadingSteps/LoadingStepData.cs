using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AAA.SourceGenerators.Common.CommonDiagnostics;

namespace AAA.LoadingGen.Generator;

public class LoadingStepData : IEquatable<LoadingStepData>, IAttributeResolver, ITypeResolver, IConstructorResolver
{
    public string Name = string.Empty;
    public string NameCamelCase = string.Empty;
    public string? TargetNamespace;
    public LoadingType LoadingType = LoadingType.Synchronous;
    public bool ExcludedByDefault = false;
    public ImmutableArray<string>? FeatureTags;
    public ImmutableArray<string>? Dependencies;
    public readonly List<string> AdditionalData = new();
    public bool IsConstructable = true;

    public LoadingStepData()
    {
    }

    public void ResolveType(INamedTypeSymbol namedTypeSymbol)
    {
        Name = namedTypeSymbol.Name;
        NameCamelCase = Name.FirstCharToLower();
        TargetNamespace = namedTypeSymbol.GetNameSpaceString();

        INamedTypeSymbol? baseType = namedTypeSymbol.BaseType;
        while (baseType != null)
        {
            AdditionalData.Add($"BaseType Name: {baseType.ToDisplayString()}");
            if (baseType.ToDisplayString() is "UnityEngine.ScriptableObject" or "UnityEngine.MonoBehaviour" or "UnityEngine.Object")
            {
                IsConstructable = false;
                break;
            }
        
            baseType = baseType.BaseType;
        }
    }

    public Diagnostic? TryResolveConstructor(IMethodSymbol ctor)
    {
        if (!IsConstructable)
            return null;

        if (ctor.Parameters.Length > 0)
            IsConstructable = false;
        return null;
    }

    public Diagnostic? TryResolveAttribute(AttributeData attributeData)
    {
        return attributeData switch
        {
            { AttributeClass.Name: "LoadingStepAttribute" } => TryResolveLoadingStepAttribute(attributeData),
            { AttributeClass.Name: "FeatureTagAttribute" } => TryResolveFeatureTagAttribute(attributeData),
            { AttributeClass.Name: "RequiresLoadingDependencyAttribute" } => TryResolveDependenciesAttribute(attributeData),
            { AttributeClass.Name: "ExcludedByDefaultAttribute" } => TryResolveExcludedAttribute(),
            _ => null
        };
    }

    private Diagnostic? TryResolveLoadingStepAttribute(AttributeData attributeData)
    {
        var targetLoadingType = attributeData.ConstructorArguments.FirstOrDefault().Value;
        if (targetLoadingType == null)
        {
            return Diagnostic.Create(IncorrectAttributeData, attributeData.ApplicationSyntaxReference?.GetSyntax().GetLocation(), attributeData.AttributeClass?.Name);
        }

        LoadingType = (LoadingType)targetLoadingType;
        return null;
    }

    private Diagnostic? TryResolveFeatureTagAttribute(AttributeData attributeData)
    {
        FeatureTags = attributeData.ConstructorArguments.FirstOrDefault().Values
            .Where(x => x.Value is not null)
            .Select(x => (string)x.Value!)
            .ToImmutableArray();
        return null;
    }

    private Diagnostic? TryResolveDependenciesAttribute(AttributeData attributeData)
    {
        Dependencies = attributeData.ConstructorArguments.FirstOrDefault().Values
            .Where(x => x.Value is not null)
            .Select(x => ((INamedTypeSymbol)x.Value!).Name)
            .ToImmutableArray();
        return null;
    }

    private Diagnostic? TryResolveExcludedAttribute()
    {
        ExcludedByDefault = true;
        return null;
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
IsConstructable: {IsConstructable}
ExcludedByDefault: {ExcludedByDefault}
{(AdditionalData.Count <= 0 ? null : AdditionalData.Aggregate("\nAdditional Data:", (s, s1) => $"{s}\n    {s1}"))}";
    }
}