using System.Collections.Generic;
using AAA.LoadingGen.Generator;
using System;

namespace AAA.LoadingGen.LoadingSteps;

class NameComparer : IEqualityComparer<LoadingStepData>
{
    public bool Equals(LoadingStepData x, LoadingStepData y)
    {
        return x.Name == y.Name;
    }

    public int GetHashCode(LoadingStepData component)
    {
        return component.Name.GetHashCode();
    }
}