using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis;

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
                if (Path.GetFileName(additionalFile.Path) != "AstModel.txt")
                    continue;

                var text = additionalFile.GetText();
                if (text == null)
                    continue;

                var textReader = new StringReader(text.ToString());

                while (textReader.ReadLine() is { } line)
                {
                    if (line.StartsWith("Begin"))
                    {
                        var baseName = textReader.ReadLine()!;
                        var writer = new StringWriter();
                        writer.WriteLine("namespace Amethyst;");
                        writer.WriteLine();
                        writer.WriteLine("public abstract class " + baseName);
                        writer.WriteLine("{");

                        var types = new Dictionary<string, string[]>();
                        while (textReader.ReadLine() is { } type && !type.StartsWith("End"))
                        {
                            string typeName = type.Split('=')[0].Trim();
                            string[] typeValues = type.Split('=')[1].Trim().Split(',');
                            types.Add(typeName, typeValues);
                        }
                        
                        writer.WriteLine("    public interface IVisitor<T>");
                        writer.WriteLine("    {");
                        foreach (var kvp in types)
                        {
                            writer.WriteLine($"        T Visit{kvp.Key}{baseName}({kvp.Key} {baseName.ToLower()});");
                        }
                        writer.WriteLine("    }");
                        writer.WriteLine();
                        
                        foreach (var kvp in types)
                        {
                            writer.WriteLine($"    public class {kvp.Key} : {baseName}");
                            writer.WriteLine("    {");
                            foreach (var value in kvp.Value)
                            {
                                var typeAndName = value.Trim();
                                writer.WriteLine($"        public required {typeAndName} {{ get; init; }}");
                            }
                            writer.WriteLine();
                            writer.WriteLine("        public override T Accept<T>(IVisitor<T> visitor)");
                            writer.WriteLine("        {");
                            writer.WriteLine($"            return visitor.Visit{kvp.Key}{baseName}(this);");
                            writer.WriteLine("        }");
                            writer.WriteLine("    }");
                            writer.WriteLine();
                        }
                        
                        writer.WriteLine("    public abstract T Accept<T>(IVisitor<T> visitor);");
                        
                        writer.WriteLine("}");
                        context.AddSource(baseName + ".g.cs", writer.ToString());
                        writer.Close();
                    }
                }
            }
        }
    }  
}