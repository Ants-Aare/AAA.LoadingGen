using System;
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
        var sequenceData = loadingSequenceDataWithDependencies.LoadingSequenceData;
        var stepDatas = loadingSequenceDataWithDependencies.LoadingSteps;
        var stringBuilder = new StringBuilder();
        try
        {
            stringBuilder.AppendGenerationWarning(GeneratorName, sequenceData.TargetNamespace + sequenceData.Name);
            stringBuilder.Append($"\n/*\n{loadingSequenceDataWithDependencies.ToString()}*/\n");
            stringBuilder.AppendLine(GenerationStringsUtility.Usings);
            foreach (var stepData in stepDatas)
                stringBuilder.AppendLine($"using {stepData.TargetNamespace};");
            
            using (new NamespaceBuilder(stringBuilder, sequenceData.TargetNamespace))
            {
                stringBuilder.AppendLine($"    public partial class {sequenceData.Name}");
            
                using (new BracketsBuilder(stringBuilder, 1))
                {
                    foreach (var stepData in stepDatas)
                        stringBuilder.AppendLine($"        readonly {stepData.Name} {stepData.NameCamelCase};");
            
                    stringBuilder.Append($"        public {sequenceData.Name}");
                    stringBuilder.AppendMethodSignature(stepDatas.Where(x => !x.IsConstructable).Select<LoadingStepData, (string, string)>(x => new(x.Name, x.NameCamelCase))
                        .GetEnumerator());
            
                    using (new BracketsBuilder(stringBuilder, 2))
                    {
                        foreach (var stepData in stepDatas)
                        {
                            if (stepData.IsConstructable)
                                stringBuilder.AppendLine($"            {stepData.NameCamelCase} = new {stepData.Name}();");
                            else
                                stringBuilder.AppendLine($"            this.{stepData.NameCamelCase} = {stepData.NameCamelCase};");
                        }
                    }
            
                    stringBuilder.AppendLine("        public async UniTask StartLoadingSequenceAsync(CancellationToken ct)");
                    using (new BracketsBuilder(stringBuilder, 2))
                    {
                        foreach (var stepData in stepDatas)
                        {
                            if (stepData.LoadingType == LoadingType.Asynchronous)
                            {
                                stringBuilder.AppendLine($"            await {stepData.NameCamelCase}.StartLoadingStepAsync(ct);");
                            }
                            else
                            {
                                stringBuilder.AppendLine($"            {stepData.NameCamelCase}.StartLoadingStep();");
                            }
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            stringBuilder.AppendLine(e.ToString());
        }
        context.AddSource($"{sequenceData.Name}.Generated.cs", stringBuilder.ToString());
    }
}