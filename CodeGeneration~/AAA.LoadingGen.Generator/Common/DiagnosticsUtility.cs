using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace AAA.SourceGenerators.Common;

public static class DiagnosticsUtility
{
    public static void ReportDiagnostics(SourceProductionContext sourceProductionContext, Diagnostic diagnostic)
        => sourceProductionContext.ReportDiagnostic(diagnostic);

    public static IncrementalValuesProvider<T> HandleDiagnostics<T>(this IncrementalValuesProvider<ResultOrDiagnostics<T>> resultsOrDiagnostics,
        IncrementalGeneratorInitializationContext context)
    {
        var diagnostics = resultsOrDiagnostics
            .Where(x => x.HasDiagnostics)
            .SelectMany((s, _) => s.Diagnostics);

        context.RegisterSourceOutput(diagnostics, ReportDiagnostics);
        
        return resultsOrDiagnostics
            .Where(x => x.Result is not null)
            .Select((s, _) => s.Result!);
    }

    public static IncrementalValueProvider<T> HandleDiagnostics<T>(this IncrementalValueProvider<ResultOrDiagnostics<T>> resultsOrDiagnostics,
        IncrementalGeneratorInitializationContext context)
    {
        var diagnostics = resultsOrDiagnostics
            .SelectMany((s, _) => s.Diagnostics);
        
        context.RegisterSourceOutput(diagnostics, ReportDiagnostics);

        return resultsOrDiagnostics.Select((x,_) => x.Result!);
    }
}