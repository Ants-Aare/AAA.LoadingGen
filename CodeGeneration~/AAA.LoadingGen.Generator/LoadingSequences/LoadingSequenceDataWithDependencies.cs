using System;
using System.Collections.Immutable;
using System.Linq;

namespace AAA.LoadingGen.Generator.LoadingSequences;

public readonly struct LoadingSequenceDataWithDependencies : IEquatable<LoadingSequenceDataWithDependencies>
{
    public readonly LoadingSequenceData LoadingSequenceData;
    public readonly ImmutableArray<LoadingStepData> LoadingSteps;

    public LoadingSequenceDataWithDependencies(LoadingSequenceData loadingSequenceData, ImmutableArray<LoadingStepData> loadingSteps)
    {
        LoadingSequenceData = loadingSequenceData;
        LoadingSteps = loadingSteps;
    }

    public bool Equals(LoadingSequenceDataWithDependencies other)
    {
        return LoadingSequenceData.Equals(other.LoadingSequenceData) && LoadingSteps.Equals(other.LoadingSteps);
    }

    public override bool Equals(object? obj)
    {
        return obj is LoadingSequenceDataWithDependencies other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (LoadingSequenceData.GetHashCode() * 397) ^ LoadingSteps.GetHashCode();
        }
    }

    public override string ToString()
    {
        return @$"{LoadingSequenceData.ToString()}
{LoadingSteps.Aggregate("LoadingSteps:\n", (s, data) => $"{s}\n{data.Name}\n")}";
    }
}