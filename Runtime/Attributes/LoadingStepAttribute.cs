using System;

namespace AAA.LoadingGen.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LoadingStepAttribute : Attribute
    {
        public LoadingStepAttribute(LoadingType loadingType, bool includedByDefault = true) { }
    }

    public enum LoadingType
    {
        Synchronous = 0,
        Asynchronous = 1,
        MultiThreaded = 2,
    }
}