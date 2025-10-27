using Amethyst.Language;
using Amethyst.Model;
using static Amethyst.Constants;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>
    ///     <p>A function declaration. It can only appear at the top level of a file within the function
    ///     registry.</p>
    ///     <p>When a function is marked as a unit test or has a <c>[no_mangle]</c> attribute, the original
    ///     name is preserved.</p>
    ///     <p>Parameters of the function are basically variables within the function body's scope and are
    ///     parsed at <see cref="VisitParameter" />.</p>
    ///     <p>Every explicitly declared function will result in a dedicated <c>.mcfunction</c> file in the
    ///     resulting datapack. This is important to know when creating APIs that need to be called from
    ///     external code.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SymbolAlreadyDeclaredException">A symbol with the same name is already declared in
    /// the current scope.</exception>
    /// <seealso cref="VisitCallExpression" />
    public override object? VisitFunctionDeclaration(AmethystParser.FunctionDeclarationContext context)
    {
        var functionName = context.IDENTIFIER().GetText();
        if (TryGetSymbol(functionName, out _, context, checkExportedSymbols: false))
        {
            throw new SymbolAlreadyDeclaredException(functionName, context);
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
            Context.Configuration.Datapack!.TickFunctions.Add(mcFunctionPath);
        }
        
        if (attributes.Contains(AttributeLoadFunction))
        {
            Context.Configuration.Datapack!.LoadFunctions.Add(mcFunctionPath);
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
        
        return null;
    }

    /// <summary>
    ///     <p>Visits a parameter list and returns an array of Variables representing the parameters.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SymbolAlreadyDeclaredException">A parameter with the same name is already declared
    /// in the current scope.</exception>
    /// <seealso cref="VisitParameter" />
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

    /// <summary><p>Visits a parameter and returns a Variable representing the parameter.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <seealso cref="VisitParameterList" />
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