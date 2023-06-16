using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeLoadingFeatureAttribute : Attribute
    {
        public ExcludeLoadingFeatureAttribute(params string[] featureTags) { }
    }
}