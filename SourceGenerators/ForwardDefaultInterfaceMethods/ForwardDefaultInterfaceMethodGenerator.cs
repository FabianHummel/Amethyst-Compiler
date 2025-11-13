using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace SourceGenerators.ForwardDefaultInterfaceMethods;

[Generator]
public class ForwardDefaultInterfaceMethodGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{SourceGenerationHelper.ForwardDefaultInterfaceMethodsAttribute}.g.cs",
            SourceText.From(SourceGenerationHelper.ForwardDefaultInterfaceMethodsAttributeCode, Encoding.UTF8)));
        
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            $"{SourceGenerationHelper.OverrideForBaseType}.g.cs",
            SourceText.From(SourceGenerationHelper.OverrideForBaseTypeCode, Encoding.UTF8)));
        
        var classSymbols = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                SourceGenerationHelper.ForwardDefaultInterfaceMethodsAttribute,
                predicate: IsSyntaxTargetForGeneration,
                transform: GetSemanticTargetForGeneration)
            .Where(static m => m is not null);
        
        var compilationAndClasses
            = classSymbols.Combine(context.CompilationProvider);
        
        context.RegisterSourceOutput(compilationAndClasses, (productionContext, source) => 
            Execute(source.Right, source.Left, productionContext));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken ct)
    {
        return node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    }
    
    private static INamedTypeSymbol GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        return context.TargetSymbol as INamedTypeSymbol;
    }
    
    private static void Execute(Compilation compilation, INamedTypeSymbol classSymbol, SourceProductionContext context)
    {
        if (GetClassGenerationModel(compilation, classSymbol) is { } model)
        {
            GenerateCodeForModel(model, context);
        }
    }
    
    private static readonly SymbolDisplayFormat _hintNameFormatting = SymbolDisplayFormat.FullyQualifiedFormat
        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.Omitted)
        .WithGenericsOptions(SymbolDisplayGenericsOptions.None);

    private static readonly Dictionary<string, InterfaceGenerationModel> _interfaceGenerationModelCache = new();

    private static ClassGenerationModel? GetClassGenerationModel(Compilation compilation, INamedTypeSymbol classSymbol)
    {
        var forwardAttribute = compilation.GetTypeByMetadataName(SourceGenerationHelper.ForwardDefaultInterfaceMethodsAttribute);
        
        var attribute = classSymbol.GetAttributes().FirstOrDefault(attributeData => 
            attributeData.AttributeClass!.Equals(forwardAttribute, SymbolEqualityComparer.Default));

        var interfaceSymbol = (INamedTypeSymbol)attribute.ConstructorArguments.First().Value!;
        if (!_interfaceGenerationModelCache.TryGetValue(interfaceSymbol.Name, out var interfaceModel))
        {
            if (!GetInterfaceGenerationModel(compilation, classSymbol, interfaceSymbol, out var model))
            {
                return null;
            }

            interfaceModel = model!.Value;
            _interfaceGenerationModelCache.Add(interfaceSymbol.Name, interfaceModel);
        }
        
        var hintName = $"{classSymbol.ToDisplayString(_hintNameFormatting)}_" +
                       $"{interfaceSymbol.ToDisplayString(_hintNameFormatting)}.g.cs";
        return new ClassGenerationModel(hintName, classSymbol, interfaceModel);
    }

    private static bool GetInterfaceGenerationModel(Compilation compilation, INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceDeclaration, out InterfaceGenerationModel? model)
    {
        model = null;
        
        var interfaceName = interfaceDeclaration.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var methods = interfaceDeclaration.GetMembers()
            .OfType<IMethodSymbol>()
            .Where(FilterInterfaceMethods)
            .Select(TransformMethodSymbol)
            .ToArray();

        var properties = interfaceDeclaration.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(FilterInterfaceProperties)
            .Select(TransformPropertySymbol)
            .ToArray();

        if (methods.Length != 0 || properties.Length != 0)
        {
            model = new InterfaceGenerationModel(interfaceName, methods, properties);
        }
        
        return model != null;

        PropertyGenerationModel TransformPropertySymbol(IPropertySymbol propertySymbol)
        {
            GetBaseTypeInfo(compilation, propertySymbol, classSymbol, 
                p => p.Parameters, out var isOverride, out var accessModifier);
            return new PropertyGenerationModel(propertySymbol, isOverride, accessModifier);
        }

        MethodGenerationModel TransformMethodSymbol(IMethodSymbol methodSymbol)
        {
            GetBaseTypeInfo(compilation, methodSymbol, classSymbol, 
                s => s.Parameters, out var isOverride, out var accessModifier);
            return new MethodGenerationModel(methodSymbol, isOverride, accessModifier);
        }
    }
    
    
    private static bool FilterInterfaceMethods(IMethodSymbol methodSymbol)
    {
        return methodSymbol is
        {
            MethodKind: MethodKind.Ordinary, 
            IsAbstract: false, 
            IsStatic: false, 
            DeclaredAccessibility: Accessibility.Public
        };
    }

    private static bool FilterInterfaceProperties(IPropertySymbol propertySymbol)
    {
        return propertySymbol is
        {
            IsAbstract: false, 
            IsStatic: false, 
            DeclaredAccessibility: Accessibility.Public
        };
    }

    private static void GenerateCodeForModel(ClassGenerationModel model, SourceProductionContext context)
    {
        context.AddSource(model.HintName, model.ToString());
    }
    
    private static void GetBaseTypeInfo<T>(Compilation compilation, T symbol, INamedTypeSymbol classSymbol, Func<T, ImmutableArray<IParameterSymbol>> getParameters, out bool isOverride, out string accessModifier)
        where T : ISymbol
    {
        isOverride = false;
        accessModifier = SyntaxFacts.GetText(Accessibility.Public);
        
        var overrideForBaseTypeAttribute = compilation.GetTypeByMetadataName(SourceGenerationHelper.OverrideForBaseType);
        
        var attributes = symbol.GetAttributes().Where(attributeData => 
            attributeData.AttributeClass!.Equals(overrideForBaseTypeAttribute, SymbolEqualityComparer.Default))
            .ToList();
        
        var overrideInformations = attributes.
            Select(attributeData => new
            {
                baseType = (INamedTypeSymbol)attributeData.ConstructorArguments[0].Value!,
                accessibilityModifier = (string)attributeData.ConstructorArguments[1].Value!
            })
            .ToArray();
        
        var baseType = classSymbol.BaseType;
        while (baseType != null)
        {
            foreach (var overrideInfo in overrideInformations)
            {
                if (baseType.Equals(overrideInfo.baseType, SymbolEqualityComparer.Default))
                {
                    isOverride = true;
                    accessModifier = overrideInfo.accessibilityModifier;
                    return;
                }
            }

            foreach (var baseMethod in baseType.GetMembers(symbol.Name).OfType<T>())
            {
                var parametersEqual = getParameters(baseMethod).AsEnumerable()
                    .SequenceEqual(getParameters(symbol), ParameterTypeComparer.Instance);
                
                if ((baseMethod.IsAbstract || baseMethod.IsVirtual) && parametersEqual)
                {
                    isOverride = true;
                    accessModifier = SyntaxFacts.GetText(baseMethod.DeclaredAccessibility);
                    return;
                }
            }

            baseType = baseType.BaseType;
        }
    }
}
