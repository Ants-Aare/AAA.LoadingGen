namespace AAA.LoadingGen.Generator;

public struct LoadingSequenceData
{
    public string Name;
    public string TargetNamespace;

    public LoadingSequenceData(string name, string targetNamespace)
    {
        Name = name;
        TargetNamespace = targetNamespace;
    }
}