using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

/// <summary>The parser parses individual source files for information that are needed to be known
/// before compilation such as which top-level statements the function exports to be imported by other
/// files.</summary>
public class Parser
{
    /// <summary>The current source file that is being parsed.</summary>
    public SourceFile SourceFile { get; set; } = null!;

    /// <summary>Parses a source file. Reads the contents of the file located at the specified
    /// <paramref name="path" />.</summary>
    /// <param name="sourceFile">The source file to parse</param>
    /// <param name="path">The actual file path where the source file is located</param>
    public void Parse(SourceFile sourceFile, string path)
    {
        var stream = File.OpenRead(path);
        Parse(sourceFile, stream);
    }

    /// <summary>Parses a source file. Uses the specified <paramref name="fileStream" /> to read the
    /// contents of the source file. This overload is used to parse internal source files that are embedded
    /// into the assembly.</summary>
    /// <param name="sourceFile">The source file to parse</param>
    /// <param name="fileStream">The file stream of source file</param>
    public void Parse(SourceFile sourceFile, Stream fileStream)
    {
        SourceFile = sourceFile;
        var inputStream = new AmethystInputStream(fileStream, sourceFile);
        var lexer = new AmethystLexer(inputStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new AmethystParser(tokenStream);
        parser.AddErrorListener(new AmethystErrorListener(this));
        parser.AddParseListener(new AmethystParseListener(this));
        parser.file();
    }
}