using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace AAA.SourceGenerators.Common;

public class ResultOrDiagnostics<T> : IEquatable<ResultOrDiagnostics<T>>
{
    public readonly T? Result;
    public readonly List<Diagnostic> Diagnostics = new();
    public bool HasDiagnostics => Diagnostics.Count > 0;

    public ResultOrDiagnostics(){}
    public ResultOrDiagnostics(T result)
    {
        Result = result;
    }

    public ResultOrDiagnostics(Diagnostic diagnostics)
    {
        Diagnostics.Add(diagnostics);
    }
    public ResultOrDiagnostics(IEnumerable<Diagnostic> diagnostics)
    {
        Diagnostics.AddRange(diagnostics);
    }

    public void TryAddDiagnostic(Diagnostic? diagnostic)
    {
        if (diagnostic is null)
            return;
        Diagnostics.Add(diagnostic);
    }
    public ResultOrDiagnostics<T> TryAddDiagnostics(IEnumerable<Diagnostic> diagnostic)
    {
        Diagnostics.AddRange(diagnostic);
        return this;
    }

    public bool Equals(ResultOrDiagnostics<T> other)
    {
        return EqualityComparer<T?>.Default.Equals(Result, other.Result) && Equals(Diagnostics, other.Diagnostics);
    }

    public override bool Equals(object? obj)
    {
        return obj is ResultOrDiagnostics<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (EqualityComparer<T?>.Default.GetHashCode(Result) * 397) ^ (Diagnostics != null ? Diagnostics.GetHashCode() : 0);
        }
    }

    public static implicit operator ResultOrDiagnostics<T>(T result) => new(result);
    public static implicit operator ResultOrDiagnostics<T>(Diagnostic diagnostic) => new(diagnostic);
}