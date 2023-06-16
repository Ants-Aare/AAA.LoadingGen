using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeLoadingStepAttribute : Attribute
    {
        public IncludeLoadingStepAttribute(params Type[] loadingStepTypes) { }
    }
}