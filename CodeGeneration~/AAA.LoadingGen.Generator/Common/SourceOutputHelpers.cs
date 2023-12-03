// using System;
// using System.Collections.Immutable;
// using System.IO;
// using System.Linq;
// using Microsoft.CodeAnalysis;
//
// namespace AAA.SourceGenerators.Common;
//
// public static class SourceOutputHelpers
// {
//     
//     public static void LogInfoToSourceGenLog(string message)
//     {
//         // Ignore IO exceptions in case there is already a lock, could use a named mutex but don't want to eat the performance cost
//         try
//         {
//             string generatedCodePath = GetGeneratedCodePath();
//             var sourceGenLogPath = Path.Combine(generatedCodePath, "SourceGen.log");
//             using var writer = File.AppendText(sourceGenLogPath);
//             writer.WriteLine(message);
//         }
//         catch (IOException) { }
//     }
// }

