using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresLoadingDependencyAttribute : Attribute
    {
        public RequiresLoadingDependencyAttribute(params Type[] dependencies) { }
    }
}