using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Model.Constants;

namespace Amethyst;

public partial class Compiler
{
    public override Symbol VisitFunctionDeclaration(AmethystParser.FunctionDeclarationContext context)
    {
        if (Context.Configuration.Datapack is null)
        {
            throw new SyntaxException($"Consider configuring a datapack in '{ConfigFile}' in order to use functions.", context);
        }
        
        var functionName = context.IDENTIFIER().GetText();
        if (EnsureSymbolIsNewOrGetRootSymbol(functionName, context, out var symbol))
        {
            return symbol;
        }

        var attributes = VisitAttributeList(context.attributeList());

        var isNoMangle = attributes.Contains(AttributeUnitTestFunction) || attributes.Contains(AttributeNoMangle);

        var parameters = Array.Empty<Variable>();

        string mcFunctionPath;
        using (this.EvaluateScoped(isNoMangle ? functionName : "_func", preserveName: isNoMangle))
        {
            mcFunctionPath = Scope.McFunctionPath;
            
            if (context.parameterList() is { } parameterListContext)
            {
                parameters = VisitParameterList(parameterListContext);
            }
        
            if (attributes.Contains(AttributeUnitTestFunction))
            {
                Context.UnitTests.Add(Scope.McFunctionPath, Scope);
            }
            
            VisitBlockInline(context.block());
        }
        
        if (attributes.Contains(AttributeTickFunction))
        {
            Context.Configuration.Datapack.TickFunctions.Add(mcFunctionPath);
        }
        
        if (attributes.Contains(AttributeLoadFunction))
        {
            Context.Configuration.Datapack.LoadFunctions.Add(mcFunctionPath);
        }
        
        var function = new Function
        {
            Attributes = attributes,
            Parameters = parameters,
            McFunctionPath = mcFunctionPath
        };

        if (!Scope.Symbols.TryAdd(functionName, function)) 
        {
           throw new SymbolAlreadyDeclaredException(functionName, context); 
        }
        
        return function;
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