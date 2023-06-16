using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class FeatureTagAttribute : Attribute
    {
        public FeatureTagAttribute(params string[] featureTags) { }
    }
}