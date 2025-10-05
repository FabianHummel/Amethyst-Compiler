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
                                                 
                                                 #nullable enable
                                                 
                                                 partial class {{symbol.Name}}
                                                 {
                                                 
                                                 """);

                    foreach (var m in @interface.GetMembers().OfType<IMethodSymbol>())
                    {
                        if (m.MethodKind != MethodKind.Ordinary) continue;
                        if (m.IsAbstract) continue;
                        if (m.DeclaredAccessibility is not Accessibility.Public) continue;
                        var returnType = m.ReturnType.ToDisplayString();
                        var returnKeyword = returnType == "void" ? "" : "return ";
                        var methodName = m.Name;
                        var overrideKeyword = HasAbstractBaseDefinition(m, symbol) ? "override " : "";
                        
                        var methodParams = string.Join(", ", m.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
                        var methodArgs = string.Join(", ", m.Parameters.Select(p => p.Name));
                        
                        sb.AppendLine($$"""
                                            public {{overrideKeyword}}{{returnType}} {{methodName}}({{methodParams}})
                                            {
                                                {{returnKeyword}}(({{interfaceName}})this).{{methodName}}({{methodArgs}});
                                            }
                                            
                                        """);
                    }
                    
                    foreach (var p in @interface.GetMembers().OfType<IPropertySymbol>())
                    {
                        if (p.DeclaredAccessibility is not Accessibility.Public) continue;
                        if (p.IsAbstract) continue;
                        var propertyType = p.Type.ToDisplayString();
                        var propertyName = p.Name;
                        var overrideKeyword = HasAbstractBaseDefinition(p, symbol) ? "override " : "";

                        if (p.GetMethod is not null)
                        {
                            sb.AppendLine($$"""
                                                public {{overrideKeyword}}{{propertyType}} {{propertyName}}
                                                {
                                                    get => (({{interfaceName}})this).{{propertyName}};
                                                }
                                                
                                            """);
                        }

                        if (p.SetMethod is not null)
                        {
                            sb.AppendLine($$"""
                                                public {{overrideKeyword}}{{propertyType}} {{propertyName}}
                                                {
                                                    set => (({{interfaceName}})this).{{propertyName}} = value;
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

    private static bool HasAbstractBaseDefinition(IMethodSymbol m, INamedTypeSymbol symbol)
    {
        bool hasAbstractBase = false;
        var baseType = symbol.BaseType;
        while (baseType != null)
        {
            if ((from baseMember in baseType.GetMembers(m.Name).OfType<IMethodSymbol>() 
                where baseMember.IsAbstract && baseMember.Parameters.Length == m.Parameters.Length 
                select !m.Parameters
                    .Where((t, i) => !SymbolEqualityComparer.Default.Equals(t.Type, baseMember.Parameters[i].Type))
                    .Any())
                .Any())
            {
                hasAbstractBase = true;
            }
            if (hasAbstractBase) break;
            baseType = baseType.BaseType;
        }
        return hasAbstractBase;
    }
    
    private static bool HasAbstractBaseDefinition(IPropertySymbol m, INamedTypeSymbol symbol)
    {
        bool hasAbstractBase = false;
        var baseType = symbol.BaseType;
        while (baseType != null)
        {
            if ((from baseMember in baseType.GetMembers(m.Name).OfType<IPropertySymbol>() 
                    where baseMember.IsAbstract && baseMember.Parameters.Length == m.Parameters.Length 
                    select !m.Parameters
                        .Where((t, i) => !SymbolEqualityComparer.Default.Equals(t.Type, baseMember.Parameters[i].Type))
                        .Any())
                .Any())
            {
                hasAbstractBase = true;
            }
            if (hasAbstractBase) break;
            baseType = baseType.BaseType;
        }
        return hasAbstractBase;
    }
}
