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
            throw new SyntaxException($"Consider configuring a datapack in '{CONFIG_FILE}' in order to use functions.", context);
        }
        
        var functionName = context.IDENTIFIER().GetText();
        if (Scope.TryGetSymbol(functionName, out _))
        {
            throw new SymbolAlreadyDeclaredException(functionName, context);
        }
        
        var attributes = VisitAttributeList(context.attributeList());

        var scopeName = "_func";
        if (attributes.Contains(ATTRIBUTE_UNIT_TEST_FUNCTION))
        {
            scopeName = functionName;
        }
        
        var scope = VisitBlockNamed(context.block(), scopeName);
        
        if (attributes.Contains(ATTRIBUTE_TICK_FUNCTION))
        {
            Context.Configuration.Datapack.TickFunctions.Add(scope.McFunctionPath);
        }
        
        if (attributes.Contains(ATTRIBUTE_LOAD_FUNCTION))
        {
            Context.Configuration.Datapack.LoadFunctions.Add(scope.McFunctionPath);
        }

        if (attributes.Contains(ATTRIBUTE_UNIT_TEST_FUNCTION))
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