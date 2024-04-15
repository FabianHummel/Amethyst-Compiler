using System.IO;
using Microsoft.CodeAnalysis;

namespace SourceGenerators
{
    [Generator]
    public class ApiSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this generator.
        }

        public void Execute(GeneratorExecutionContext context)
        {
            
        }
    }  
}