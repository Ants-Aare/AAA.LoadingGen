using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using AAA.LoadingGen.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static AAA.SourceGenerators.Common.CommonDiagnostics;

namespace AAA.SourceGenerators.Common;

public static class CommonTransforms
{
    public static ResultOrDiagnostics<T> TransformResolved<T>(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        where T : new()
    {
        // return Diagnostic.Create(ExceptionOccured, context.Node.GetLocation(), nameof(TransformResolved), "test", "test");
                // nameof(TransformResolved), context.Node.ToFullString(), e.ToString());)
        var instance = new T();
        var resultOrDiagnostics = new ResultOrDiagnostics<T>(instance);
        try
        {
            // throw new Exception("Test");
            var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
            
            if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax, cancellationToken) is not INamedTypeSymbol namedTypeSymbol)
                return Diagnostic.Create(CommonDiagnostics.NamedTypeSymbolNotFound, classDeclarationSyntax.GetLocation(), classDeclarationSyntax.Identifier.Text);
            
            var diagnostics = Resolve(instance, namedTypeSymbol, cancellationToken);
            resultOrDiagnostics.TryAddDiagnostics(diagnostics);
            return resultOrDiagnostics;
        }
        catch (Exception e)
        {
            var diagnostic = Diagnostic.Create(CommonDiagnostics.ExceptionOccured, context.Node.GetLocation(),
                nameof(TransformResolved), context.Node.ToFullString(), e.ToString());
            resultOrDiagnostics.TryAddDiagnostic(diagnostic);
        }
        
        return resultOrDiagnostics;
    }

    // public static ResultOrDiagnostics<T> TransformResolved<T>(this INamedTypeSymbol namedTypeSymbol, CancellationToken cancellationToken)
    //     where T : new()
    // {
    //     var instance = new T();
    //     var resultOrDiagnostics = new ResultOrDiagnostics<T>(instance);
    //     try
    //     {
    //         resultOrDiagnostics.TryAddDiagnostics(Resolve(instance, namedTypeSymbol, cancellationToken));
    //     }
    //     catch (Exception e)
    //     {
    //         var diagnostic = Diagnostic.Create(CommonDiagnostics.ExceptionOccured, Location.None,
    //             nameof(TransformResolved), namedTypeSymbol.ToDisplayString(), e.ToString());
    //         resultOrDiagnostics.TryAddDiagnostic(diagnostic);
    //     }
    //
    //     return resultOrDiagnostics;
    // }

    // public static ResultOrDiagnostics<ImmutableArray<T>> TransformResolved<T>(Compilation compilation, CancellationToken ct, Func<INamedTypeSymbol,bool> filter)
    //     where T : new()
    // {
    //     var diagnostics = new List<Diagnostic>();
    //     var allInstances = new List<T>();
    //     try
    //     {
    //         var stack = new Stack<INamespaceSymbol>();
    //         stack.Push(compilation.GlobalNamespace);
    //         while (stack.Count > 0)
    //         {
    //             ct.ThrowIfCancellationRequested();
    //             foreach (var member in stack.Pop().GetMembers())
    //             {
    //                 ct.ThrowIfCancellationRequested();
    //                 switch (member)
    //                 {
    //                     case INamespaceSymbol namespaceSymbol:
    //                         stack.Push(namespaceSymbol);
    //                         break;
    //                     case INamedTypeSymbol namedTypeSymbol:
    //                     {
    //                         if (!filter(namedTypeSymbol))
    //                             continue;
    //
    //                         var instance = new T();
    //                         diagnostics.AddRange(Resolve<T>(instance, namedTypeSymbol, ct));
    //                         allInstances.Add(instance);
    //                         break;
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     catch (Exception e)
    //     {
    //         var diagnostic = Diagnostic.Create(CommonDiagnostics.ExceptionOccured, Location.None,
    //             nameof(TransformResolved), "Compilation", e.ToString());
    //         diagnostics.Add(diagnostic);
    //     }
    //
    //     if (allInstances.Count == 0)
    //         return new ResultOrDiagnostics<ImmutableArray<T>>(diagnostics);
    //     
    //     allInstances.Sort();
    //     var immutableArray = allInstances.ToImmutableArray();
    //     return new ResultOrDiagnostics<ImmutableArray<T>>(immutableArray).TryAddDiagnostics(diagnostics);
    // }

    public static List<Diagnostic> Resolve<T>(T instance, INamedTypeSymbol namedTypeSymbol, CancellationToken cancellationToken)
    {
        var diagnostics = new List<Diagnostic>();

        if (instance is ITypeResolver typeResolver)
            typeResolver.ResolveType(namedTypeSymbol);
        
        if (instance is IAttributeResolver attributeResolver)
        {
            var x = ResolveAttributes(attributeResolver, namedTypeSymbol, cancellationToken);
            if (x != null) diagnostics.Add(x);
        }

        if (instance is IConstructorResolver constructorResolver)
        {
            var x = ResolveConstructors(constructorResolver, namedTypeSymbol, cancellationToken);
            if (x != null) diagnostics.Add(x);
        }

        return diagnostics;
    }

    public static Diagnostic? ResolveAttributes<T>(this T instance, INamedTypeSymbol namedTypeSymbol, CancellationToken ct)
        where T : IAttributeResolver
    {
        var attributeDatas = namedTypeSymbol.GetAttributes();
        foreach (var attributeData in attributeDatas)
        {
            ct.ThrowIfCancellationRequested();
            var diagnostic = instance.TryResolveAttribute(attributeData);
            if (diagnostic is not null)
                return diagnostic;
        }

        return null;
    }

    public static Diagnostic? ResolveConstructors<T>(this T instance, INamedTypeSymbol namedTypeSymbol, CancellationToken ct)
        where T : IConstructorResolver
    {
        foreach (var constructor in namedTypeSymbol.Constructors)
        {
            if (constructor is null)
                continue;
            ct.ThrowIfCancellationRequested();

            var diagnostic = instance.TryResolveConstructor(constructor);
            if (diagnostic is not null)
                return diagnostic;
        }

        return null;
    }
}

public interface IConstructorResolver
{
    public Diagnostic? TryResolveConstructor(IMethodSymbol ctor);
}

public interface IAttributeResolver
{
    Diagnostic? TryResolveAttribute(AttributeData attributeData);
}

public interface ITypeResolver
{
    public void ResolveType(INamedTypeSymbol namedTypeSymbol);
}