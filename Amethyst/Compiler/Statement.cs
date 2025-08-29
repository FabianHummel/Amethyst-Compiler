using System.Diagnostics;
using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitStatement(AmethystParser.StatementContext context)
    {
        if (context.declaration() is { } declaration)
        {
            return VisitDeclaration(declaration);
        }
        if (context.for_statement() is { } forStatement)
        {
            return VisitFor_statement(forStatement);
        }
        if (context.while_statement() is { } whileStatement)
        {
            return VisitWhile_statement(whileStatement);
        }
        if (context.foreach_statement() is { } foreachStatement)
        {
            return VisitForeach_statement(foreachStatement);
        }
        if (context.if_statement() is { } ifStatement)
        {
            return VisitIf_statement(ifStatement);
        }
        if (context.debug_statement() is { } debugStatement)
        {
            return VisitDebug_statement(debugStatement);
        }
        if (context.comment_statement() is { } commentStatement)
        {
            return VisitComment_statement(commentStatement);
        }
        if (context.return_statement() is { } returnStatement)
        {
            return VisitReturn_statement(returnStatement);
        }
        if (context.break_statement() is { } breakStatement)
        {
            return VisitBreak_statement(breakStatement);
        }
        if (context.continue_statement() is { } continueStatement)
        {
            return VisitContinue_statement(continueStatement);
        }
        if (context.block() is { } block)
        {
            return VisitBlock(block);
        }
        if (context.expression_statement() is { } expressionStatement)
        {
            return VisitExpression_statement(expressionStatement);
        }
        
        throw new UnreachableException();
    }
}