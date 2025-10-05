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
        if (Scope.TryGetSymbol(functionName, out _))
        {
            throw new SymbolAlreadyDeclaredException(functionName, context);
        }
        
        var attributes = VisitAttributeList(context.attributeList());

        var scopeName = "_func";
        if (attributes.Contains(AttributeUnitTestFunction))
        {
            scopeName = functionName;
        }
        
        var scope = VisitBlockNamed(context.block(), scopeName);
        
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
            Context.UnitTests.Add(scope.McFunctionPath, scope);
        }
        
        Scope.Symbols.Add(functionName, new Function
        {
            Attributes = attributes
        });
        
        return null;
    }
}