using System.IO;
using Microsoft.CodeAnalysis;

namespace SourceGenerators
{
    [Generator]
    public class AbstractResultGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required
        }
        
        private readonly string[] _types = new[]
        {
            "IntResult",
            "DecResult",
            "StringResult",
            "BoolResult",
            "ArrayResult",
            "ObjectResult",
            "AnyArrayResult",
            "AnyObjectResult"
        };
        
        private string GetMemberName(string type) => char.ToLower(type[0]) + type.Substring(1);

        private readonly (string, string)[] _operations = new[]
        {
            ("add", "+"),
            ("subtract", "-"),
            ("multiply", "*"),
            ("divide", "/"),
            ("modulo", "%")
        };
        
        public void Execute(GeneratorExecutionContext context)
        {
            // @formatter:off
            var writer = new StringWriter();
            writer.WriteLine("using System.Diagnostics;");
            writer.WriteLine("using Amethyst.Model;");
            writer.WriteLine("using Antlr4.Runtime;");
            writer.WriteLine("using Amethyst.Model.Types;");
            writer.WriteLine();
            
            writer.WriteLine("namespace Amethyst;");
            writer.WriteLine();
            writer.WriteLine("public partial class Compiler"); 
            writer.WriteLine("{");
            
            foreach (var (operation, symbol) in _operations)
            {
                writer.WriteLine($"    public virtual AbstractResult Visit_{operation}(AbstractResult lhs, AbstractResult rhs, ParserRuleContext context)");
                writer.WriteLine( "    {");
                foreach (var lhs in _types)
                {
                    writer.WriteLine($"        if (lhs is {lhs} {GetMemberName(lhs)}Lhs)");
                    writer.WriteLine( "        {");
                    foreach (var rhs in _types)
                    {
                        writer.WriteLine($"            if (rhs is {rhs} {GetMemberName(rhs)}Rhs)");
                        writer.WriteLine( "            {");
                        writer.WriteLine($"                return Visit_{operation}({GetMemberName(lhs)}Lhs, {GetMemberName(rhs)}Rhs, context);");
                        writer.WriteLine( "            }");
                    }
                    writer.WriteLine( "        }");
                }
                writer.WriteLine();
                writer.WriteLine("        throw new UnreachableException();");
                writer.WriteLine("    }");
                writer.WriteLine();
            }
            
            writer.WriteLine("}");
            
            // @formatter:on
            
            context.AddSource("ArithmeticCompiler.g.cs", writer.ToString());
            
            writer.Close();
            
            writer = new StringWriter();
            
            writer.WriteLine("using System.Diagnostics;");
            writer.WriteLine("using Amethyst.Model;");
            writer.WriteLine("using Antlr4.Runtime;");
            writer.WriteLine("using Amethyst.Model.Types;");
            writer.WriteLine();
            
            writer.WriteLine("namespace Amethyst;");
            writer.WriteLine();
            writer.WriteLine("public interface IArithmeticBase");
            writer.WriteLine("{");

            foreach (var (operation, symbol) in _operations)
            {
                foreach (var lhs in _types)
                {
                    foreach (var rhs in _types)
                    {
                        writer.WriteLine($"    AbstractResult Visit_{operation}({lhs} lhs, {rhs} rhs, ParserRuleContext context)");
                        writer.WriteLine( "    {");
                        writer.WriteLine($"        throw new SyntaxException($\"Operation {{lhs.DataType}} {symbol} {{rhs.DataType}} is not permitted\", context);");
                        writer.WriteLine( "    }");
                        writer.WriteLine();
                    }
                }
            }
            
            writer.WriteLine("}");
            
            context.AddSource("ArithmeticBase.g.cs", writer.ToString());
            
            writer.Close();
        }
    }
}