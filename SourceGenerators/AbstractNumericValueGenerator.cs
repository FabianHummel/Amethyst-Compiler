using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.CodeAnalysis;

namespace SourceGenerators;

[Generator]
public class AbstractNumericValueGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // No initialization required
    }

    public void Execute(GeneratorExecutionContext context)
    {
        using var writer = new StringWriter();
        
        var sb = new StringBuilder();

        foreach (var (visitor, @operator) in new List<Tuple<string, string>>
        {
            new("Add", "ADD"),
            new("Subtract", "SUBTRACT"),
            new("Multiply", "MULTIPLY"),
            new("Divide", "DIVIDE"),
            new("Modulo", "MODULO")
        })
        {
            sb.AppendLine($$"""
                            
                            protected override AbstractValue Visit{{visitor}}(AbstractValue av)
                            {
                                if (av is AbstractNumericValue rhs)
                                {
                                    return Calculate(this, rhs, ArithmeticOperator.{{@operator}});
                                }
                                
                                return base.Visit{{visitor}}(av);
                            }
                        """);
        }
        
        foreach (var (visitor, @operator) in new List<Tuple<string, string>>
        {
            new("LessThan", "LESS_THAN"),
            new("LessThanOrEqual", "LESS_THAN_OR_EQUAL"),
            new("GreaterThan", "GREATER_THAN"),
            new("GreaterThanOrEqual", "GREATER_THAN_OR_EQUAL")
        })
        {
            sb.AppendLine($$"""
                            
                            protected override AbstractValue Visit{{visitor}}(AbstractValue av)
                            {
                                if (av is AbstractNumericValue rhs)
                                {
                                    return Calculate(this, rhs, ComparisonOperator.{{@operator}});
                                }
                                
                                return base.Visit{{visitor}}(av);
                            }
                        """);
        }

        writer.WriteLine($$"""
                         using Amethyst.Model;

                         namespace Amethyst;

                         public abstract partial class AbstractNumericValue
                         {{{sb}}}
                         """);
        
        context.AddSource("AbstractNumericValue.g.cs", writer.ToString());
    }
}