// using System.Text;
// using Microsoft.CodeAnalysis;
//
// namespace AAA.LoadingGen.Generator;
//
// [Generator]
// public class TestGen : ISourceGenerator
// {
//     public static readonly StringBuilder StringBuilder = new StringBuilder();
//     public void Initialize(GeneratorInitializationContext context)
//     {
//         
//     }
//
//     public void Execute(GeneratorExecutionContext context)
//     {
//         if (StringBuilder.Length == 0)
//             return;
//         StringBuilder.Append("public partial class CreateGameManagerLoadingStep{}");
//         context.AddSource("Test.Generated.cs", StringBuilder.ToString());
//         StringBuilder.Clear();
//     }
// }