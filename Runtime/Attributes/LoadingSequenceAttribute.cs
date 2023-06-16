using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LoadingSequenceAttribute : Attribute
    {
        public LoadingSequenceAttribute(LoadingSteps includedLoadingSteps) { }
    }

    public enum LoadingSteps
    {
        None = 0,
        All = 1,
    }
}