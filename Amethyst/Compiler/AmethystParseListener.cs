using Amethyst.Language;
using Amethyst.Model;

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
        var path = context.String_Literal(0).GetText();
        if (path == null)
        {
            throw new SyntaxException("Expected resource path for the import path.", context);
        }

        var symbols = context.String_Literal()
            .Skip(1)
            .Select(s => s.GetText()!)
            .ToList();

        if (symbols.Count == 0)
        {
            throw new SyntaxException("Expected at least one symbol to import.", context);
        }

        foreach (var symbolName in symbols)
        {
            if (!Parser.SourceFile!.ExportedSymbols.ContainsKey(symbolName) &&
                !Parser.SourceFile!.ImportedSymbols.TryAdd(symbolName, path))
            {
                throw new SyntaxException($"'{symbolName}' is already declared in this scope.", context);
            }
        }
    }
    
    public override void ExitDeclaration(AmethystParser.DeclarationContext context)
    {
        if (Parser.RegistryName != Constants.DATAPACK_FUNCTIONS_DIRECTORY)
        {
            throw new SyntaxException("Runtime declarations must go inside the 'function' registry.", context);
        }

        string? symbolName = null;
        if (context.function_declaration() is { } functionDeclarationContext)
        {
            symbolName = functionDeclarationContext.identifier().GetText();
        }
        else if (context.variable_declaration() is { } variableDeclarationContext)
        {
            symbolName = variableDeclarationContext.identifier().GetText();
        }
        else if (context.record_declaration() is { } recordDeclarationContext)
        {
            symbolName = recordDeclarationContext.identifier().GetText();
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
        var fnName = context.identifier().GetText();
        if (fnName == null)
        {
            throw new SyntaxException("Expected function name.", context);
        }

        var attributesListContext = context.attribute_list();
        var attributes = attributesListContext.attribute()
            .Select(attributeContext => attributeContext.identifier().GetText())
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