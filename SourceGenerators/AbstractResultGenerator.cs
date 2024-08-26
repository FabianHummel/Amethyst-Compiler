using System.IO;
using Microsoft.CodeAnalysis;

namespace SourceGenerators;

[Generator]
public class AbstractResultGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }
        
    private readonly string[] _types = new[]
    {
        "IntegerResult",
        "DecimalResult",
        "StringResult",
        "BooleanResult",
        "ArrayResult",
        "ObjectResult",
        "DynArrayResult",
        "DynObjectResult"
    };
        
    private string GetMemberName(string type) => char.ToLower(type[0]) + type.Substring(1);

    private readonly (string, string)[] _operations = new[]
    {
        ("Add", "+"),
        ("Subtract", "-"),
        ("Multiply", "*"),
        ("Divide", "/"),
        ("Modulo", "%")
    };
        
    public void Execute(GeneratorExecutionContext context)
    {
        var writer = new StringWriter();
            
        writer.WriteLine("""
                         using System.Diagnostics;
                         using Amethyst.Model;
                         using Antlr4.Runtime;

                         namespace Amethyst;

                         public abstract partial class AbstractResult
                         {
                         """);

        foreach (var (operation, symbol) in _operations)
        {
            writer.WriteLine($$"""
                                   public static AbstractResult operator {{symbol}} (AbstractResult lhs, AbstractResult rhs)
                                   {
                               """);

            foreach (var rhs in _types)
            {
                writer.WriteLine($$"""
                                           if (rhs is {{rhs}} {{GetMemberName(rhs)}})
                                           {
                                               return lhs.Visit{{operation}}({{GetMemberName(rhs)}});
                                           }
                                   """);
            }
                
            writer.WriteLine("""
                                     throw new UnreachableException();
                                 }
                                 
                             """);

            foreach (var rhs in _types)
            {
                writer.WriteLine($$"""
                                       protected virtual AbstractResult Visit{{operation}}({{rhs}} rhs)
                                       {
                                           throw new SyntaxException($"Operation {this.DataType} {{symbol}} {rhs.DataType} not permitted", this.Context);
                                       }

                                   """);
            }
        }

        writer.WriteLine("}");
            
        context.AddSource("AbstractResultOperations.g.cs", writer.ToString());
            
        writer.Close();
    }
}