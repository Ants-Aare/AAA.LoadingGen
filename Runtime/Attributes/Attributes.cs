using System;
using System.Diagnostics;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class Attributes : Attribute
    {
        public Attributes(params string[] featureTags) { }
    }
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class ExcludeLoadingStepAttribute : Attribute
    {
        public ExcludeLoadingStepAttribute(params Type[] loadingStepTypes) { }
    }
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class ExcludeLoadingFeatureAttribute : Attribute
    {
        public ExcludeLoadingFeatureAttribute(params string[] featureTags) { }
    }
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class FeatureTagAttribute : Attribute
    {
        public FeatureTagAttribute(params string[] featureTags) { }
    }
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class IncludeLoadingFeatureAttribute : Attribute
    {
        public IncludeLoadingFeatureAttribute(params string[] featureTags) { }
    }
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class IncludeLoadingStepAttribute : Attribute
    {
        public IncludeLoadingStepAttribute(params Type[] loadingStepTypes) { }
    }
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class LoadingSequenceAttribute : Attribute
    {
        public LoadingSequenceAttribute(LoadingStepsFilter loadingStepsFilter) { }
    }

    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class LoadingStepAttribute : Attribute
    {
        public LoadingStepAttribute(LoadingType loadingType, bool includedByDefault = true) { }
    }

    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class RequiresLoadingDependencyAttribute : Attribute
    {
        public RequiresLoadingDependencyAttribute(params Type[] dependencies) { }
    }
    [AttributeUsage(AttributeTargets.Class)][Conditional("LOADINGGEN_INCLUDE_ATTRIBUTES")]
    public class SubstituteLoadingStepAttribute : Attribute
    {
        public SubstituteLoadingStepAttribute(Type[] targetTypes, Type[] replacementTypes) { }
    }
}