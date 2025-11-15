using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGenerators.ForwardDefaultInterfaceMethods;

public readonly record struct ClassGenerationModel
{
    public readonly string HintName;
    public readonly INamedTypeSymbol Class;
    public readonly InterfaceGenerationModel Interface;

    public ClassGenerationModel(string hintName, INamedTypeSymbol @class, InterfaceGenerationModel @interface)
    {
        HintName = hintName;
        Class = @class;
        Interface = @interface;
    }

    public override string ToString()
    {
        return $$"""
                 namespace {{Class.ContainingNamespace.Name}};

                 #nullable enable

                 partial class {{Class.Name}}
                 {
                 {{Interface.ToCode()}}}
                 """;
    }
}

public readonly record struct InterfaceGenerationModel
{
    public readonly string Name;
    public readonly MethodGenerationModel[] Methods;
    public readonly PropertyGenerationModel[] Properties;

    public InterfaceGenerationModel(string name, MethodGenerationModel[] methods, PropertyGenerationModel[] properties)
    {
        Name = name;
        Methods = methods;
        Properties = properties;
    }

    public string ToCode()
    {
        var interfaceName = Name;
        
        var codeParts = new List<string>();

        if (Methods.Length > 0)
        {
            codeParts.Add(string.Join("\n", Methods
                .Select(m => m.ToCode(interfaceName))
                .Select(code => $"{code}\n")));
        }

        if (Properties.Length > 0)
        {
            codeParts.Add(string.Join("\n", Properties
                .SelectMany(p => p.ToCode(interfaceName))
                .Select(code => $"{code}\n")));
        }
        
        return string.Join("\n", codeParts);
    }
}

public readonly record struct MethodGenerationModel
{
    public readonly IMethodSymbol Method;
    public readonly bool IsOverride;
    public readonly string AccessModifier;
    
    public MethodGenerationModel(IMethodSymbol method, bool isOverride, string accessModifier)
    {
        Method = method;
        IsOverride = isOverride;
        AccessModifier = accessModifier;
    }

    public string ToCode(string interfaceName)
    {
        var overrideKeyword = IsOverride ? "override " : "";
        var returnKeyword = Method.ReturnsVoid ? "" : "return ";
        var methodParams = string.Join(", ", Method.Parameters.Select(p => $"{p.Type.ToDisplayString()} {p.Name}"));
        var methodArgs = string.Join(", ", Method.Parameters.Select(p => p.Name));
        
        return $$"""
                     {{AccessModifier}} {{overrideKeyword}}{{Method.ReturnType}} {{Method.Name}}({{methodParams}})
                     {
                         {{returnKeyword}}(({{interfaceName}})this).{{Method.Name}}({{methodArgs}});
                     }
                 """;
    }
}

public readonly record struct PropertyGenerationModel
{
    public readonly IPropertySymbol Property;
    public readonly bool IsOverride;
    public readonly string AccessModifier;
    
    public PropertyGenerationModel(IPropertySymbol property, bool isOverride, string accessModifier)
    {
        Property = property;
        IsOverride = isOverride;
        AccessModifier = accessModifier;
    }

    public IEnumerable<string> ToCode(string interfaceName)
    {
        var overrideKeyword = IsOverride ? "override " : "";

        if (Property.GetMethod != null)
        {
            yield return $$"""
                               {{AccessModifier}} {{overrideKeyword}}{{Property.Type}} {{Property.Name}}
                               {
                                   get => (({{interfaceName}})this).{{Property.Name}};
                               }
                           """;
        }

        if (Property.SetMethod != null)
        {
            yield return $$"""
                               {{AccessModifier}} {{overrideKeyword}}{{Property.Type}} {{Property.Name}}
                               {
                                   set => (({{interfaceName}})this).{{Property.Name}} = value;
                               }
                           """;
        }
    }
}