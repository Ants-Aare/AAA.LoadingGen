#nullable enable
using System;

namespace AAA.SourceGenerators.Common;

public static class ExceptionExtensions
{
    public static string ToUnityPrintableString(this Exception exception) => exception.ToString().Replace(Environment.NewLine, " |--| ");
}