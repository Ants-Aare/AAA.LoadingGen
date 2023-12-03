using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace AAA.SourceGenerators.Common;

public readonly struct ResultOrDiagnostics<T> : IEquatable<ResultOrDiagnostics<T>>
{
    public readonly T? Result;
    public readonly Diagnostic? Diagnostic;

    public ResultOrDiagnostics(T result)
    {
        Result = result;
    }

    public ResultOrDiagnostics(Diagnostic diagnostic)
    {
        Diagnostic = diagnostic;
    }

    public bool Equals(ResultOrDiagnostics<T> other)
    {
        return EqualityComparer<T?>.Default.Equals(Result, other.Result) && Equals(Diagnostic, other.Diagnostic);
    }

    public override bool Equals(object? obj)
    {
        return obj is ResultOrDiagnostics<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (EqualityComparer<T?>.Default.GetHashCode(Result) * 397) ^ (Diagnostic != null ? Diagnostic.GetHashCode() : 0);
        }
    }

    public static implicit operator ResultOrDiagnostics<T>(T result) => new ResultOrDiagnostics<T>(result);
    public static implicit operator ResultOrDiagnostics<T>(Diagnostic diagnostic) => new ResultOrDiagnostics<T>(diagnostic);
}