using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Constants;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitFunctionDeclaration(AmethystParser.FunctionDeclarationContext context)
    {
        if (Context.Configuration.Datapack is null)
        {
            throw new SyntaxException($"Consider configuring a datapack in '{ConfigFile}' in order to use functions.", context);
        }
        
        var functionName = context.IDENTIFIER().GetText();
        if (TryGetSymbol(functionName, out _, context, checkExportedSymbols: false))
        {
            throw new SymbolAlreadyDeclaredException(functionName, context);
        }
        
        var attributes = VisitAttributeList(context.attributeList());

        var parameters = Array.Empty<Variable>();
        var scope = VisitBlockNamed(context.block(), "_func", () =>
        {
            if (context.parameterList() is { } parameterListContext)
            {
                parameters = VisitParameterList(parameterListContext);
            }
        });
        
        if (attributes.Contains(AttributeTickFunction))
        {
            Context.Configuration.Datapack.TickFunctions.Add(scope.McFunctionPath);
        }
        
        if (attributes.Contains(AttributeLoadFunction))
        {
            Context.Configuration.Datapack.LoadFunctions.Add(scope.McFunctionPath);
        }

        if (attributes.Contains(AttributeUnitTestFunction))
        {
            var mcFunctionScope = new Scope
            {
                Parent = scope.Parent,
                Context = scope.Context,
                Name = functionName
            };
            
            Context.UnitTests.Add(mcFunctionScope.McFunctionPath, scope);
        }

        var function = new Function
        {
            Attributes = attributes,
            Parameters = parameters,
            Scope = scope
        };

        if (!Scope.Symbols.TryAdd(functionName, function)) 
        {
           throw new SymbolAlreadyDeclaredException(functionName, context); 
        }
        
        return null;
    }
    
    public override Variable[] VisitParameterList(AmethystParser.ParameterListContext context)
    {
        var parameters = new List<Variable>();
        foreach (var parameterContext in context.parameter())
        {
            var parameter = VisitParameter(parameterContext);
            
            if (!Scope.Symbols.TryAdd(parameter.Name, parameter))
            {
                throw new SymbolAlreadyDeclaredException(parameter.Name, context);
            }
            
            parameters.Add(parameter);
        }
        
        return parameters.ToArray();
    }
    
    public override Variable VisitParameter(AmethystParser.ParameterContext context)
    {
        var datatype = VisitType(context.type());
        var numericLocation = ++StackPointer;
        
        return new Variable
        {
            Name = context.IDENTIFIER().GetText(),
            Datatype = datatype,
            Attributes = VisitAttributeList(context.attributeList()),
            Location = new Location
            {
                Name = numericLocation.ToString(),
                DataLocation = datatype.DataLocation
            }
        };
    }
}