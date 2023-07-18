using System;
using System.Text;

namespace AAA.LoadingGen.Generator;

public static class GenerationStringsUtility
{
    public const string GenerationWarning = @"// <auto-generated>
//     This code was generated by AAA.UnityOpenInternals.OpenInternalsSourceGenerator
//     Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
";

    public const string Usings =
@"using AAA.LoadingGen.Runtime;
using System;
using ";
}

public abstract class StringBuilderGenerationBase : IDisposable
{
    protected readonly StringBuilder StringBuilder;

    protected StringBuilderGenerationBase(StringBuilder stringBuilder)
    {
        StringBuilder = stringBuilder;
    }

    public void Dispose()
    {
        AppendBottom();
    }

    protected abstract void AppendBottom();
}

public class NamespaceBuilder : StringBuilderGenerationBase
{
    private readonly string? _targetNamespace;

    public NamespaceBuilder(StringBuilder stringBuilder, string? targetNamespace) : base(stringBuilder)
    {
        _targetNamespace = targetNamespace;
        if (_targetNamespace != null)
            StringBuilder.AppendLine($"namespace {_targetNamespace}\n{{");
    }

    protected override void AppendBottom()
    {
        if (_targetNamespace != null)
            StringBuilder.AppendLine("}");
    }
}

//TODO: implement proper solution once I have time for it
public class BracketsBuilder : StringBuilderGenerationBase
{
    private readonly int _indentLevel;

    public BracketsBuilder(StringBuilder stringBuilder, int indentLevel) : base(stringBuilder)
    {
        _indentLevel = indentLevel;
        switch (_indentLevel)
        {
            case 0:
                StringBuilder.AppendLine("{");
                break;
            case 1:
                StringBuilder.AppendLine("    {");
                break;
            default:
                StringBuilder.AppendLine("        {");
                break;
        }
    }

    protected override void AppendBottom()
    {
        switch (_indentLevel)
        {
            case 0:
                StringBuilder.AppendLine("}");
                break;
            case 1:
                StringBuilder.AppendLine("    }");
                break;
            default:
                StringBuilder.AppendLine("        }");
                break;
        }
    }
}