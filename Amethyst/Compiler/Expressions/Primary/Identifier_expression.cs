using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitIdentifier_expression(AmethystParser.Identifier_expressionContext context)
    {
        if (context.namespace_access() is not { } namespaceAccessContext)
        {
            throw new UnreachableException();
        }
        
        var (scope, identifierContext) = VisitNamespace_access(namespaceAccessContext);

        var identifier = identifierContext.IDENTIFIER().Symbol.Text;
        
        if (scope.TryGetVariable(identifier, out var variable))
        {
            if (variable.DataType.Modifier is { } modifier)
            {
                return modifier switch
                {
                    Modifier.Array => new ArrayResult { Compiler = this, Context = identifierContext, Location = variable.Location, BasicType = variable.DataType.BasicType },
                    Modifier.Object => new ObjectResult { Compiler = this, Context = identifierContext, Location = variable.Location, BasicType = variable.DataType.BasicType },
                    _ => throw new UnreachableException()
                };
            }

            return variable.DataType.BasicType switch
            {
                BasicType.Int => new IntegerResult { Compiler = this, Context = identifierContext, Location = variable.Location },
                BasicType.Dec => new DecimalResult { Compiler = this, Context = identifierContext, Location = variable.Location },
                BasicType.Bool => new BooleanResult { Compiler = this, Context = identifierContext, Location = variable.Location },
                BasicType.String => new StringResult { Compiler = this, Context = identifierContext, Location = variable.Location },
                BasicType.Array => new DynArrayResult { Compiler = this, Context = identifierContext, Location = variable.Location, },
                BasicType.Object => new DynObjectResult { Compiler = this, Context = identifierContext, Location = variable.Location },
                _ => throw new UnreachableException()
            };
        }
        
        if (scope.TryGetRecord(identifier, out var record))
        {
            throw new NotImplementedException();
        }
        
        throw new SyntaxException($"The variable or record '{identifier}' does not exist in the current context.", identifierContext);
    }
}