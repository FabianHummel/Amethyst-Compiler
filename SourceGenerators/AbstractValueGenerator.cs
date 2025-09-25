using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGenerators;

[Generator]
public class AbstractValueGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }
        
    private readonly string[] _types = {
        "AbstractInteger",
        "AbstractDecimal",
        "AbstractString",
        "AbstractBoolean",
        "AbstractArray",
        "AbstractObject"
    };

    private readonly (string, string)[] _arithmeticOperators = {
        ("Add", "+"),
        ("Subtract", "-"),
        ("Multiply", "*"),
        ("Divide", "/"),
        ("Modulo", "%")
    };

    private readonly (string, string)[] _comparisonOperators =
    {
        ("LessThan", "<"),
        ("LessThanOrEqual", "<="),
        ("GreaterThan", ">"),
        ("GreaterThanOrEqual", ">=")
    };
        
    private static string GetMemberName(string type)
    {
        return char.ToLower(type[0]) + type.Substring(1);
    }

    public void Execute(GeneratorExecutionContext context)
    {
        using var writer = new StringWriter();
        
        writer.WriteLine("""
                         using System.Diagnostics;
                         using Amethyst.Model;
                         using Antlr4.Runtime;

                         namespace Amethyst;

                         public abstract partial class AbstractValue
                         {
                         """);

        foreach (var (@operator, symbol) in _arithmeticOperators.Concat(_comparisonOperators))
        {
            writer.WriteLine($$"""
                                   public static AbstractValue operator {{symbol}} (AbstractValue lhs, AbstractValue rhs)
                                   {
                                        return lhs.Visit{{@operator}}(rhs);
                                   }
                                   
                                   protected virtual AbstractValue Visit{{@operator}}(AbstractValue rhs)
                                   {
                               """);
                
            foreach (var rhs in _types)
            {
                writer.WriteLine($$"""
                                           if (rhs is {{rhs}} {{GetMemberName(rhs)}})
                                           {
                                               return Visit{{@operator}}({{GetMemberName(rhs)}});
                                           }
                                   """);
            }
                
            writer.WriteLine($$"""
                                       throw new SyntaxException($"Operation {this.DataType} {{symbol}} {rhs.DataType} is not defined.", this.Context);
                                   }

                               """);

            foreach (var rhs in _types)
            {
                writer.WriteLine($$"""
                                       protected virtual AbstractValue Visit{{@operator}}({{rhs}} rhs)
                                       {
                                           throw new SyntaxException($"Operation {this.DataType} {{symbol}} {rhs.DataType} is not permitted.", this.Context);
                                       }

                                   """);
            }
        }

        writer.WriteLine("}");
            
        context.AddSource("AbstractValue.g.cs", writer.ToString());
    }
}