using Amethyst.Language;
using Amethyst.Model;

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

    public override void ExitPreprocessorFromImportDeclaration(AmethystParser.PreprocessorFromImportDeclarationContext context)
    {
        if (context.preprocessorResourceLiteral() is not { } resourceLiteral)
        {
            return;
        }

        if (resourceLiteral is not AmethystParser.PreprocessorRegularResourceLiteralContext regularResourceLiteralContext)
        {
            throw new SyntaxException("Import declarations must not contain expressions in resource literals.", context);
        }
        
        var symbols = context.IDENTIFIER()
            .Select(s => s.GetText())
            .ToList();

        foreach (var symbolName in symbols)
        {
            var resource = new Resource(regularResourceLiteralContext.GetText()[1..^1]);
            
            if (!Parser.SourceFile.ExportedSymbols.ContainsKey(symbolName) &&
                !Parser.SourceFile.ImportedSymbols.TryAdd(symbolName, resource))
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
}