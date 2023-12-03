using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AAA.LoadingGen.Generator;
using AAA.LoadingGen.Generator.LoadingSequences;
using AAA.SourceGenerators;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;

namespace AAA.LoadingGen.LoadingSequences;

public class LoadingSequenceGenerator
{
    public static readonly string GeneratorName = typeof(LoadingSequenceGenerator).FullName!;

    public static void GenerateOutput(SourceProductionContext context, LoadingSequenceDataWithDependencies loadingSequenceDataWithDependencies)
    {
        var loadingSequenceData = loadingSequenceDataWithDependencies.LoadingSequenceData;

        var stringBuilder = new StringBuilder().AppendGenerationWarning(GeneratorName, loadingSequenceData.Name);
        stringBuilder.Append($"\n/*\n{loadingSequenceDataWithDependencies.ToString()}*/\n");

        stringBuilder.AppendLine(GenerationStringsUtility.Usings);

        using (new NamespaceBuilder(stringBuilder, loadingSequenceData.TargetNamespace))
        {
            stringBuilder.AppendLine($"    public partial class {loadingSequenceData.Name}");

            using (new BracketsBuilder(stringBuilder, 1))
            {
                stringBuilder.AppendLine("        public void StartLoadingSequence(Action onCompleted = null)");
                using (new BracketsBuilder(stringBuilder, 2))
                {
                    foreach (var stepData in loadingSequenceDataWithDependencies.LoadingSteps)
                    {
                        if (stepData.LoadingType == LoadingType.Asynchronous)
                        {
                            stringBuilder.AppendLine()
                        }
                    }
                }
            }
        }

        context.AddSource($"{loadingSequenceData.Name}.Generated.cs", stringBuilder.ToString());
    }
}