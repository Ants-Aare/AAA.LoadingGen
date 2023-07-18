using Microsoft.CodeAnalysis;

namespace AAA.LoadingGen.Generator;

public static class LoadingGenDiagnostics
{

    public static void ReportDiagnostics(SourceProductionContext sourceProductionContext, Diagnostic diagnostic)
        => sourceProductionContext.ReportDiagnostic(diagnostic);

    public static readonly DiagnosticDescriptor ClassNotPartial = new (id: "LOADINGGEN001", 
        title: "Target Class is not marked as partial",
        messageFormat: "Cannot generate code for class {0} if is not marked as 'partial'",
        category: "UserInputRequired",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
    
    public static readonly DiagnosticDescriptor NamedTypeSymbolNotFound = new(id: "LOADINGGEN002",
        title: "NamedTypeSymbol was not found in compilation",
        messageFormat: "Error finding the type '{0}' in the compilation. This should not occur.",
        category: "Unexpected",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    public static readonly DiagnosticDescriptor ExceptionOccured = new(id: "LOADINGGEN003",
        title: "Exception was caught while Generating",
        messageFormat: "Exception occured in {0}. Message: {1}.",
        category: "Unexpected",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    public static readonly DiagnosticDescriptor IncorrectAttributeData = new(id: "LOADINGGEN004",
        title: "Failed to Parse Attribute Data",
        messageFormat: "Could not parse the data of Attribute {0} in {1}",
        category: "Unexpected",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static IncrementalValuesProvider<T> HandleDiagnostics<T>(this IncrementalValuesProvider<ResultOrDiagnostics<T>> resultsOrDiagnostics, IncrementalGeneratorInitializationContext context)
    {
        var diagnostics = resultsOrDiagnostics
            .Where(x => x.Diagnostic is not null)
            .Select((s, _) => s.Diagnostic!);
        
        context.RegisterSourceOutput(diagnostics, ReportDiagnostics);

        return resultsOrDiagnostics
            // .Where(x=>x is not null)
            .Select((s, _) => s.Result!);
    }
}
