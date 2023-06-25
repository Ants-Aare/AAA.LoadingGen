using Microsoft.CodeAnalysis;

namespace AAA.LoadingGen.Generator;

public class LoadingGenDiagnostics
{
    public static void ReportDiagnostics(SourceProductionContext sourceProductionContext, Diagnostic diagnostic)
        => sourceProductionContext.ReportDiagnostic(diagnostic);

    public static readonly DiagnosticDescriptor NamedTypeSymbolNotFound = new(id: "LOADINGGEN002",
        title: "NamedTypeSymbol was not found in compilation",
        messageFormat: "Error finding the type '{0}' in the compilation. This should not occur.",
        category: "ComponentGenerator",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
