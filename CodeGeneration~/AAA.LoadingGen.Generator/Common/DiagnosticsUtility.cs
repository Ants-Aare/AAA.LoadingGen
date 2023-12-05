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
            .Where(x => x.Diagnostic is not null)
            .Select((s, _) => s.Diagnostic!);

        context.RegisterSourceOutput(diagnostics, ReportDiagnostics);

        context.RegisterSourceOutput(diagnostics.Collect(), ReportDiagnosticsToComments);
        return resultsOrDiagnostics
            // .Where(x=>x is not null)
            .Select((s, _) => s.Result!);
    }

    private static void ReportDiagnosticsToComments(SourceProductionContext context, ImmutableArray<Diagnostic> immutableArray)
    {
        var stringBuilder = new StringBuilder("namespace AAA.Core{public partial class SourceGeneratorDiagnostics{}}");
        foreach (var diagnostic in immutableArray)
        {
            stringBuilder.Append("\n//").Append(diagnostic.ToString());
        }

        context.AddSource($"Diagnostics.g.cs", stringBuilder.ToString());
    }
}

