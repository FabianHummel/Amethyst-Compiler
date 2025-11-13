namespace SourceGenerators.ForwardDefaultInterfaceMethods;

public static class SourceGenerationHelper
{
    public const string ForwardDefaultInterfaceMethodsAttribute = "Amethyst.Utility.ForwardDefaultInterfaceMethodsAttribute";
    
    public const string ForwardDefaultInterfaceMethodsAttributeCode =
        """
        namespace Amethyst.Utility
        {
            [System.AttributeUsage(System.AttributeTargets.Class)]
            public sealed class ForwardDefaultInterfaceMethodsAttribute : System.Attribute
            {
                public ForwardDefaultInterfaceMethodsAttribute(Type @interface)
                {
                }
            }
        }
        """;
    
    public const string OverrideForBaseType = "Amethyst.Utility.OverrideForBaseTypeAttribute";
    
    public const string OverrideForBaseTypeCode =
        """
        namespace Amethyst.Utility
        {
            [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Property)]
            public sealed class OverrideForBaseTypeAttribute : System.Attribute
            {
                public OverrideForBaseTypeAttribute(Type @baseType, string accessibilityModifier)
                {
                }
            }
        }
        """;
}