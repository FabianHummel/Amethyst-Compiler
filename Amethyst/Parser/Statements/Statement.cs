using Amethyst.Model;

namespace Amethyst.Parser;

public partial class Parser
{
    private Stmt Statement()
    {
        if (Match(TokenType.VAR))
        {
            return VarDeclaration();
        }
        
        if (Match(TokenType.FOR))
        {
            return ForStatement();
        }

        if (Match(TokenType.IF))
        {
            return IfStatement();
        }

        if (Match(TokenType.DEBUG))
        {
            return DebugStatement();
        }

        if (Match(TokenType.COMMENT))
        {
            return CommentStatement();
        }

        if (Match(TokenType.BREAK))
        {
            return BreakStatement();
        }

        if (Match(TokenType.CONTINUE))
        {
            return ContinueStatement();
        }

        if (Match(TokenType.RETURN))
        {
            return ReturnStatement();
        }

        if (Match(TokenType.WHILE))
        {
            return WhileStatement();
        }

        if (Match(TokenType.LEFT_BRACE))
        {
            return Block();
        }

        return ExpressionStatement();
    }
}