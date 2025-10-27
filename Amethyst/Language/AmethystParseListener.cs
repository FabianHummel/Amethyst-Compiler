using Amethyst.Language;

namespace Amethyst;

/// <summary>Listener for parsing Amethyst source files. Handles symbol registration so that source
/// files can import symbols from other files.</summary>
public class AmethystParseListener : AmethystParserBaseListener
{
    private Parser Parser { get; }
    private int _depth;
    
    public AmethystParseListener(Parser parser)
    {
        Parser = parser;
    }

    public override void EnterBlock(AmethystParser.BlockContext context)
    {
        _depth++;
    }
    
    public override void ExitBlock(AmethystParser.BlockContext context)
    {
        _depth--;
        if (_depth < 0)
        {
            throw new SyntaxException("Mismatched block depth.", context);
        }
    }

    public override void ExitPreprocessorFromDeclaration(AmethystParser.PreprocessorFromDeclarationContext context)
    {
        if (context.RESOURCE_LITERAL() is not { } resourceLiteral)
        {
            return;
        }
        
        var symbols = context.IDENTIFIER()
            .Select(s => s.GetText())
            .ToList();

        foreach (var symbolName in symbols)
        {
            if (!Parser.SourceFile.ExportedSymbols.ContainsKey(symbolName) &&
                !Parser.SourceFile.ImportedSymbols.TryAdd(symbolName, resourceLiteral.GetText()[1..^1]))
            {
                throw new SymbolAlreadyDeclaredException(symbolName, context);
            }
        }
    }
    
    public override void ExitDeclaration(AmethystParser.DeclarationContext context)
    {
        if (Parser.SourceFile.Registry != Constants.DatapackFunctionsDirectory)
        {
            throw new SyntaxException($"Runtime declarations must go inside the '{Constants.DatapackFunctionsDirectory}' registry.", context);
        }

        string? symbolName = null;
        if (context.functionDeclaration() is { } functionDeclarationContext)
        {
            symbolName = functionDeclarationContext.IDENTIFIER().GetText();
        }
        else if (context.variableDeclaration() is { } variableDeclarationContext)
        {
            symbolName = variableDeclarationContext.IDENTIFIER().GetText();
        }
        else if (context.recordDeclaration() is { } recordDeclarationContext)
        {
            symbolName = recordDeclarationContext.IDENTIFIER().GetText();
        }
        
        if (symbolName is null)
        {
            throw new SyntaxException("Could not determine symbol name for declaration.", context);
        }
        
        if (_depth > 0)
        {
            return; // Only register top-level declarations
        }
        
        if (!Parser.SourceFile.ExportedSymbols.TryAdd(symbolName, context) && 
            !Parser.SourceFile.ImportedSymbols.ContainsKey(symbolName))
        {
            throw new SymbolAlreadyDeclaredException(symbolName, context);
        }
    }

    public override void ExitFunctionDeclaration(AmethystParser.FunctionDeclarationContext context)
    {
        var fnName = context.IDENTIFIER().GetText();

        var attributesListContext = context.attributeList();
        var attributes = attributesListContext.attribute()
            .Select(attributeContext => attributeContext.IDENTIFIER().GetText())
            .ToHashSet();
        
        var entryPointAttributes = new HashSet<string>
        {
            Constants.AttributeLoadFunction,
            Constants.AttributeTickFunction
        };
        if (attributes.Overlaps(entryPointAttributes))
        {
            Parser.SourceFile.EntryPointFunctions.Add(fnName, context);
        }
    }
}