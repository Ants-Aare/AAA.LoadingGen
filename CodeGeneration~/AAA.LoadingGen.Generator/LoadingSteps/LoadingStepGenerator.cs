using System;
using System.Text;
using AAA.LoadingGen.Generator;
using AAA.SourceGenerators;
using AAA.SourceGenerators.Common;
using Microsoft.CodeAnalysis;

namespace AAA.LoadingGen.LoadingSteps;

public class LoadingStepGenerator
{
    public static readonly string GeneratorName = typeof(LoadingStepGenerator).FullName!;

    public static void GenerateOutput(SourceProductionContext context, LoadingStepData loadingStepData)
    {
        var stringBuilder = new StringBuilder();

        try
        {
            stringBuilder.AppendGenerationWarning(GeneratorName, loadingStepData.TargetNamespace + loadingStepData.Name);
            stringBuilder.Append($"\n/*\n{loadingStepData.ToString()}\n*/");
            stringBuilder.AppendLine(GenerationStringsUtility.Usings);
            using (new NamespaceBuilder(stringBuilder, loadingStepData.TargetNamespace))
            {
                stringBuilder.AppendLine($"    public partial class {loadingStepData.Name} : {loadingStepData.LoadingType switch
                {
                    LoadingType.Synchronous => "ILoadingStep",
                    LoadingType.Asynchronous => "IAsyncLoadingStep",
                    _ => throw new ArgumentOutOfRangeException()
                }}");
                using (new BracketsBuilder(stringBuilder, 1))
                {
                    switch (loadingStepData.LoadingType)
                    {
                        case LoadingType.Synchronous:
                            stringBuilder.AppendLine("        public void StartLoadingStep()");
                            using (new BracketsBuilder(stringBuilder, 2))
                            {
                                stringBuilder.AppendLine("            Load();");
                            }

                            break;
                        case LoadingType.Asynchronous:
                            stringBuilder.AppendLine(
                                @"        public float currentProgress = 0f;
        public async UniTask StartLoadingStepAsync(CancellationToken cancellationToken)
        {
            await LoadAsync(cancellationToken);
            currentProgress = 1f;
        }");
                            break;
                        default:
                            break;
                    }
                    
                    // using (new BracketsBuilder(stringBuilder, 2))
                    // {
                    //     
                    // }
                }
            }
        }
        catch (Exception e)
        {
            stringBuilder.AppendLine(e.ToString());
        }

        context.AddSource($"{loadingStepData.Name}.Generated.cs", stringBuilder.ToString());
    }
}