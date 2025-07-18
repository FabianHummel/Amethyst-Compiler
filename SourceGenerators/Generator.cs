using System.IO;
using System.Linq;
using CaseConverter;
using Microsoft.CodeAnalysis;

namespace SourceGenerators;

[Generator]
public class Generator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }
        
    private readonly string[] _types = {
        "IntegerResult",
        "DecimalResult",
        "StringResult",
        "BooleanResult",
        "StaticArrayResult",
        "StaticObjectResult",
        "DynArrayResult",
        "DynObjectResult"
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
        using (var writer = new StringWriter())
        {
            writer.WriteLine("""
                             using System.Diagnostics;
                             using Amethyst.Model;
                             using Antlr4.Runtime;

                             namespace Amethyst;

                             public abstract partial class RuntimeValue
                             {
                             """);

            foreach (var (@operator, symbol) in _arithmeticOperators.Concat(_comparisonOperators))
            {
                writer.WriteLine($$"""
                                       public static RuntimeValue operator {{symbol}} (RuntimeValue lhs, RuntimeValue rhs)
                                       {
                                   """);

                foreach (var rhs in _types)
                {
                    writer.WriteLine($$"""
                                               if (rhs is {{rhs}} {{GetMemberName(rhs)}})
                                               {
                                                   return lhs.Visit{{@operator}}({{GetMemberName(rhs)}});
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
                                           protected virtual RuntimeValue Visit{{@operator}}({{rhs}} rhs)
                                           {
                                               throw new SyntaxException($"Operation {this.DataType} {{symbol}} {rhs.DataType} not permitted", this.Context);
                                           }

                                       """);
                }
            }

            writer.WriteLine("}");
            
            context.AddSource("RuntimeValueOperations.g.cs", writer.ToString());
        }

        using (var writer = new StringWriter())
        {
            writer.WriteLine("""
                             using Amethyst.Model;
                             using Amethyst.Utility;

                             namespace Amethyst;

                             public abstract partial class NumericBase : RuntimeValue
                             {
                             """);
        
            string[] _numberTypes = {
                "IntegerResult",
                "DecimalResult",
                "BooleanResult"
            };

            foreach (var (@operator, _) in _arithmeticOperators)
            {
                foreach (var numberType in _numberTypes)
                {
                    writer.WriteLine($$"""
                                           protected override NumericBase Visit{{@operator}}({{numberType}} rhs)
                                           {
                                               return Calculate(rhs, ArithmeticOperator.{{@operator.ToSnakeCase().ToUpper()}});
                                           }
                                           
                                       """);
                }
            }
            
            foreach (var (@operator, _) in _comparisonOperators)
            {
                foreach (var numberType in _numberTypes)
                {
                    writer.WriteLine($$"""
                                           protected override NumericBase Visit{{@operator}}({{numberType}} rhs)
                                           {
                                               return Calculate(rhs, ComparisonOperator.{{@operator.ToSnakeCase().ToUpper()}});
                                           }
                                           
                                       """);
                }
            }
            
            writer.WriteLine("}");
        
            context.AddSource("NumericBaseOperations.g.cs", writer.ToString());
        }
    }
}