using Microsoft.CodeAnalysis;

namespace AAA.SourceGenerators.Common;

public static class CommonDiagnostics
{
    public static readonly DiagnosticDescriptor ClassNotPartial = new(id: "COM001",
        title: "Target Class is not marked as partial",
        messageFormat: "Cannot generate code for class {0} if is not marked as 'partial'",
        category: "UserInputRequired",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NamedTypeSymbolNotFound = new(id: "COM002",
        title: "NamedTypeSymbol was not found in compilation",
        messageFormat: "Error finding the type '{0}' in the compilation. This should not occur.",
        category: "Unexpected",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ExceptionOccured = new(id: "COM003",
        title: "Exception was caught while Generating",
        messageFormat: "Exception occured in {0}. Message: {1}.",
        category: "Unexpected",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor IncorrectAttributeData = new(id: "COM004",
        title: "Failed to Parse Attribute Data",
        messageFormat: "Could not parse the data of Attribute {0} in {1}",
        category: "Unexpected",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}