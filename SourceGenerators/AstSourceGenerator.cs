using System.IO;
using Microsoft.CodeAnalysis;
using Tommy;

namespace SourceGenerators
{
    /// <summary>
    /// A sample source generator that creates C# classes based on the text file (in this case, Domain Driven Design ubiquitous language registry).
    /// When using a simple text file as a baseline, we can create a non-incremental source generator.
    /// </summary>
    [Generator]
    public class AstSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this generator.
        }

        public void Execute(GeneratorExecutionContext context)
        {
            // If you would like to put some data to non-compilable file (e.g. a .txt file), mark it as an Additional File.
            
            // Go through all files marked as an Additional File in file properties.
            foreach (var additionalFile in context.AdditionalFiles)
            {
                if (additionalFile == null)
                    continue;
                
                // Check if the file name is the specific file that we expect.
                if (Path.GetFileName(additionalFile.Path) != "AstModel.toml")
                    continue;

                var text = additionalFile.GetText();
                if (text == null)
                    continue;

                var textReader = new StringReader(text.ToString());
                
                var table = TOML.Parse(textReader);
                
                foreach (var key in table.Keys)
                {
                    var writer = new StringWriter();
                    writer.WriteLine("namespace Amethyst;"); 
                    writer.WriteLine();
                    writer.WriteLine("public abstract class " + key);
                    writer.WriteLine("{");
                    
                    writer.WriteLine("    public interface IVisitor<T>");
                    writer.WriteLine("    {");
                    
                    foreach (var type in table[key].Keys)
                    {
                        writer.WriteLine($"        T Visit{type}{key}({type} {key.ToLower()});");
                    }
                    
                    writer.WriteLine("    }");
                    writer.WriteLine();
                    
                    foreach (var type in table[key].Keys)
                    {
                        writer.WriteLine($"    public class {type} : {key}");
                        writer.WriteLine("    {");
                        foreach (var field in table[key][type]["Fields"].AsArray)
                        {
                            writer.WriteLine($"        public required {field} {{ get; init; }}");
                        }
                        writer.WriteLine();
                        writer.WriteLine("        public override T Accept<T>(IVisitor<T> visitor)");
                        writer.WriteLine("        {");
                        writer.WriteLine($"            return visitor.Visit{type}{key}(this);");
                        writer.WriteLine("        }");
                        writer.WriteLine();
                        writer.WriteLine("        public override string ToString()");
                        writer.WriteLine("        {");
                        writer.WriteLine($"            return $\"{table[key][type]["ToString"].AsString}\";");
                        writer.WriteLine("        }");
                        writer.WriteLine("    }");
                        writer.WriteLine();
                    }
                    
                    writer.WriteLine("    public abstract T Accept<T>(IVisitor<T> visitor);");
                    writer.WriteLine("}");
                    context.AddSource(key + ".g.cs", writer.ToString());
                    writer.Close();
                }
            }
        }
    }  
}