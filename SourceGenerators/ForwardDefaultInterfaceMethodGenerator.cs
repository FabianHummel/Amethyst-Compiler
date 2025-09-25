using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SourceGenerators;

[Generator]
public class ForwardDefaultInterfaceMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is ClassDeclarationSyntax cds && cds.Modifiers.Any(m => m.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword)),
                static (ctx, _) => (ClassDeclarationSyntax)ctx.Node
            )
            .Where(c => c is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, pair) =>
        {
            var (compilation, classes) = pair;
            var forwardAttr = compilation.GetTypeByMetadataName("Amethyst.Utility.ForwardDefaultInterfaceMethodsAttribute");
            if (forwardAttr is null) return;

            foreach (var cls in classes)
            {
                var model = compilation.GetSemanticModel(cls.SyntaxTree);
                if (model.GetDeclaredSymbol(cls) is not INamedTypeSymbol symbol) continue;

                foreach (var @interface in symbol.AllInterfaces)
                {
                    if (!@interface.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, forwardAttr)))
                    {
                        continue;
                    }
                    
                    var interfaceName = @interface.ToDisplayString();

                    var sb = new StringBuilder($$"""
                                                 namespace {{symbol.ContainingNamespace.ToDisplayString()}};
                                                 
                                                 partial class {{symbol.Name}} : {{interfaceName}}
                                                 {
                                                 
                                                 """);

                    foreach (var m in @interface.GetMembers().OfType<IMethodSymbol>())
                    {
                        if (m.MethodKind != MethodKind.Ordinary) continue;
                        if (m.IsAbstract) continue;
                        if (m.DeclaredAccessibility is not Accessibility.Public) continue;
                        // Check if there is an abstract method in any base class with the same name and parameters
                        bool hasAbstractBase = false;
                        var baseType = symbol.BaseType;
                        while (baseType != null)
                        {
                            foreach (var baseMember in baseType.GetMembers(m.Name).OfType<IMethodSymbol>())
                            {
                                if (baseMember.IsAbstract && baseMember.Parameters.Length == m.Parameters.Length)
                                {
                                    // Check parameter types match
                                    bool paramsMatch = !m.Parameters
                                        .Where((t, i) => !SymbolEqualityComparer.Default.Equals(t.Type, baseMember.Parameters[i].Type))
                                        .Any();
                                    
                                    if (paramsMatch)
                                    {
                                        hasAbstractBase = true;
                                        break;
                                    }
                                }
                            }
                            if (hasAbstractBase) break;
                            baseType = baseType.BaseType;
                        }
                        var returnType = m.ReturnType.ToDisplayString();
                        var methodName = m.Name;
                        var @params = string.Join(", ", m.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
                        var args = string.Join(", ", m.Parameters.Select(p => p.Name));

                        if (hasAbstractBase)
                        {
                            sb.AppendLine($$"""
                                            public override {{returnType}} {{methodName}}({{@params}})
                                            {
                                                return (({{interfaceName}})this).{{methodName}}({{args}});
                                            }
                                            
                                        """);
                        }
                        else
                        {
                            sb.AppendLine($$"""
                                            public {{returnType}} {{methodName}}({{@params}})
                                            {
                                                return (({{interfaceName}})this).{{methodName}}({{args}});
                                            }
                                            
                                        """);
                        }
                    }
                    
                    sb.AppendLine("}");
                    spc.AddSource($"{symbol.Name}_Forwards.g.cs", sb.ToString());
                }
            }
        });
    }
}
