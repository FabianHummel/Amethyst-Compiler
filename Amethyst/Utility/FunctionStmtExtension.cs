using Amethyst.Model;

namespace Amethyst.Utility;

public static class FunctionStmtExtension
{
    public static string GetCallablePath(this Stmt.Function function)
    {
        return function.Name.Lexeme;
    }
}