using System;
using System.Linq;
using System.Text;
using AAA.LoadingGen.Generator;
using AAA.SourceGenerators;
using Microsoft.CodeAnalysis;

namespace AAA.LoadingGen.LoadingSteps;

public class LoadingStepGenerator
{
    public static void GenerateOutput(SourceProductionContext sourceProductionContext, LoadingStepData loadingStepData)
    {
        var stringBuilder = new StringBuilder(GenerationStringsUtility.GenerationWarning);

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
                // stringBuilder.Append($"/*");
                switch (loadingStepData.LoadingType)
                {
                    case LoadingType.Synchronous:
                        stringBuilder.AppendLine("        public void StartLoadingStep()");
                        using (new BracketsBuilder(stringBuilder, 2))
                        {
                            stringBuilder.AppendLine("        public void StartLoadingStep()");
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

                // stringBuilder.Append("*/");

                // using (new BracketsBuilder(stringBuilder, 2))
                // {
                //     
                // }
            }
        }


        sourceProductionContext.AddSource($"{loadingStepData.Name}.Generated.cs", stringBuilder.ToString());
    }
}