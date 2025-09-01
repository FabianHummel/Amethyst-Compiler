using Amethyst.Language;

namespace Amethyst;

public class AmethystParseListener : AmethystBaseListener
{
    private Parser Parser { get; }
    
    public AmethystParseListener(Parser parser)
    {
        Parser = parser;
    }
    
    public override void ExitFrom(AmethystParser.FromContext context)
    {
        if (context.RESOURCE_LITERAL() is not { } resourcePath)
        {
            return;
        }

        var symbols = context.IDENTIFIER()
            .Select(s => s.GetText())
            .ToList();

        foreach (var symbolName in symbols)
        {
            if (!Parser.SourceFile!.ExportedSymbols.ContainsKey(symbolName) &&
                !Parser.SourceFile!.ImportedSymbols.TryAdd(symbolName, resourcePath.GetText()[1..^1]))
            {
                throw new SyntaxException($"'{symbolName}' is already declared in this scope.", context);
            }
        }
    }
    
    public override void ExitDeclaration(AmethystParser.DeclarationContext context)
    {
        if (Parser.RegistryName != Constants.DATAPACK_FUNCTIONS_DIRECTORY)
        {
            throw new SyntaxException($"Runtime declarations must go inside the '{Constants.DATAPACK_FUNCTIONS_DIRECTORY}' registry.", context);
        }

        string? symbolName = null;
        if (context.function_declaration() is { } functionDeclarationContext)
        {
            symbolName = functionDeclarationContext.IDENTIFIER().GetText();
        }
        else if (context.variable_declaration() is { } variableDeclarationContext)
        {
            symbolName = variableDeclarationContext.IDENTIFIER().GetText();
        }
        else if (context.record_declaration() is { } recordDeclarationContext)
        {
            symbolName = recordDeclarationContext.IDENTIFIER().GetText();
        }
        
        if (symbolName is null)
        {
            throw new SyntaxException("Could not determine symbol name for declaration.", context);
        }
        
        if (!Parser.SourceFile!.ExportedSymbols.TryAdd(symbolName, context) && 
            !Parser.SourceFile!.ImportedSymbols.ContainsKey(symbolName))
        {
            throw new SyntaxException($"Symbol '{symbolName}' is already declared in this scope.", context);
        }
    }

    public override void ExitFunction_declaration(AmethystParser.Function_declarationContext context)
    {
        var fnName = context.IDENTIFIER().GetText();
        if (fnName == null)
        {
            throw new SyntaxException("Expected function name.", context);
        }

        var attributesListContext = context.attribute_list();
        var attributes = attributesListContext.attribute()
            .Select(attributeContext => attributeContext.IDENTIFIER().GetText())
            .ToHashSet();
        
        var entryPointAttributes = new HashSet<string>
        {
            Constants.ATTRIBUTE_LOAD_FUNCTION,
            Constants.ATTRIBUTE_TICK_FUNCTION
        };
        if (attributes.Overlaps(entryPointAttributes))
        {
            Parser.SourceFile!.EntryPointFunctions.Add(fnName, context);
        }
    }
}