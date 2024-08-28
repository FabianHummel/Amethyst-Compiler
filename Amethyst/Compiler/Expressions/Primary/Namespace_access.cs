using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override Tuple<Scope, AmethystParser.IdentifierContext> VisitNamespace_access(AmethystParser.Namespace_accessContext context)
    {
        if (context.identifier() is not { } identifierContexts)
        {
            throw new UnreachableException();
        }

        if (identifierContexts.Length == 1)
        {
            return new Tuple<Scope, AmethystParser.IdentifierContext>(Scope, identifierContexts[0]);
        }

        var namespaceName = identifierContexts[0].IDENTIFIER().Symbol.Text;
        
        if (Context.Namespaces.Find(ns => ns.Scope.Name == namespaceName) is not {} @namespace)
        {
            throw new SyntaxException($"Namespace '{namespaceName}' not found", identifierContexts[0]);
        }
        
        return new Tuple<Scope, AmethystParser.IdentifierContext>(@namespace.Scope, identifierContexts[1]);
    }
}