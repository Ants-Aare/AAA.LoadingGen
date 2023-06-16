using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeLoadingFeatureAttribute : Attribute
    {
        public IncludeLoadingFeatureAttribute(params string[] featureTags) { }
    }
}