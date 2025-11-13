using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace SourceGenerators.ValueGenerators;

[Generator]
public class AbstractPreprocessorValueGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }
        
    private readonly string[] _types = {
        "PreprocessorBoolean",
        "PreprocessorDecimal",
        "PreprocessorInteger",
        "PreprocessorResource",
        "PreprocessorString"
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

                         public abstract partial class AbstractPreprocessorValue
                         {
                         """);

        foreach (var (@operator, symbol) in _arithmeticOperators.Concat(_comparisonOperators))
        {
            writer.WriteLine($$"""
                                   public static AbstractPreprocessorValue operator {{symbol}} (AbstractPreprocessorValue lhs, AbstractPreprocessorValue rhs)
                                   {
                                        return lhs.Visit{{@operator}}(rhs);
                                   }
                                   
                                   protected virtual AbstractPreprocessorValue Visit{{@operator}}(AbstractPreprocessorValue rhs)
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
                                       throw new InvalidOperationException($"Operation {this.Datatype} {{symbol}} {rhs.Datatype} is not defined.");
                                   }

                               """);

            foreach (var rhs in _types)
            {
                writer.WriteLine($$"""
                                       protected virtual AbstractPreprocessorValue Visit{{@operator}}({{rhs}} rhs)
                                       {
                                           throw new SyntaxException($"Operation {this.Datatype} {{symbol}} {rhs.Datatype} is not permitted.", this.Context);
                                       }

                                   """);
            }
        }

        writer.WriteLine("}");
            
        context.AddSource("AbstractPreprocessorValue.g.cs", writer.ToString());
    }
}