using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst.Utility;

public class AmethystInputStream : AntlrInputStream
{
    public override string SourceName { get; }

    public AmethystInputStream(Stream input, SourceFile sourceFile) : base(input)
    {
        SourceName = sourceFile.Name;
    }
}