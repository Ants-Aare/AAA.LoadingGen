using System;
using System.Collections.Generic;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeLoadingStepAttribute : Attribute
    {
        public ExcludeLoadingStepAttribute(params Type[] loadingStepTypes) { }
    }
}