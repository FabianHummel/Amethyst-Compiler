using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Utility method that parses any declaration and returns a <see cref="Symbol" /> that can be
    ///     used to gather more information about the declared type.</p>
    ///     <p><inheritdoc /></p></summary>
    public override Symbol VisitDeclaration(AmethystParser.DeclarationContext context)
    {
        return (Symbol)base.VisitDeclaration(context)!;
    }
}