using Antlr4.Runtime;

namespace Amethyst.Utility;

public class AmethystInputStream : AntlrInputStream
{
    public override string SourceName { get; }

    public AmethystInputStream(Stream input, string sourceName) : base(input)
    {
        SourceName = sourceName;
    }
}