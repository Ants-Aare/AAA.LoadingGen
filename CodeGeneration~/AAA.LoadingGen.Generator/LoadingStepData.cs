using Microsoft.CodeAnalysis;

namespace AAA.LoadingGen.Generator;

public struct LoadingStepData
{
    public string Name;
    public string TargetNamespace;
    public string LoadingType;
    public string[]? FeatureTags;
    public string[]? Dependencies;

    public LoadingStepData(string name, string targetNamespace, string loadingType, string[]? featureTags, string[]? dependencies)
    {
        Name = name;
        TargetNamespace = targetNamespace;
        LoadingType = loadingType;
        FeatureTags = featureTags;
        Dependencies = dependencies;
    }
}