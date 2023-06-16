using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SubstituteLoadingStepAttribute : Attribute
    {
        public SubstituteLoadingStepAttribute(Type[] targetTypes, Type[] replacementTypes) { }
    }
}