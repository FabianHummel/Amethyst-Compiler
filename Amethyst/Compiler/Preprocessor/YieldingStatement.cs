using Amethyst.Language;
using System.Collections.Generic;

namespace Amethyst;

public partial class Compiler
{
    public new IReadOnlyList<T> VisitPreprocessorYieldingStatement<T>(AmethystParser.PreprocessorYieldingStatementContext context)
    {
        using var scope = CreateYieldingScope();
        Visit(context);
        return scope.Result;
    }
}