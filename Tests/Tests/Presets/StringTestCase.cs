namespace Tests.Presets;

public class StringTestCase
{
    public required string Name { get; init; }

    public required string Value { get; init; }

    public string? VariableValue { get; init; }
    
    public required string Expected { get; init; }

    public override string ToString() => Name;
    
    public static readonly IEnumerable<StringTestCase> Regular = new[]
    {
        new StringTestCase
        {
            Name = "basic",
            Value = "Basic String",
            Expected = "Basic String"
        },
        new StringTestCase
        {
            Name = "special_characters",
            Value = "!$%&/(=?+*#'",
            Expected = "!$%&/(=?+*#'"
        },
        new StringTestCase
        {
            Name = "escaping",
            Value = "\\\"\'",
            Expected = @"\""'"
        },
        new StringTestCase
        {
            Name = "unicode",
            Value = "\u1234",
            Expected = "\u1234"
        },
        new StringTestCase
        {
            Name = "empty",
            Value = "",
            Expected = ""
        },
        new StringTestCase
        {
            Name = "whitespace",
            Value = " \n\t\r",
            Expected = " \n\t\r"
        }
    };

    public static readonly IEnumerable<StringTestCase> Interpolated = new[]
    {
        new StringTestCase
        {
            Name = "regular",
            Value = "Regular String",
            Expected = "Regular String"
        },
        new StringTestCase
        {
            Name = "basic_interpolation",
            Value = "{123}",
            Expected = "123"
        },
        new StringTestCase
        {
            Name = "nested_interpolation",
            Value = "Hello {\"World\"}",
            Expected = "Hello World"
        },
        new StringTestCase
        {
            Name = "variable_interpolation_integer",
            Value = "{value}",
            VariableValue = "123",
            Expected = "123"
        },
        new StringTestCase
        {
            Name = "variable_interpolation_boolean",
            Value = "{value}",
            VariableValue = "true",
            Expected = "true"
        },
        new StringTestCase
        {
            Name = "variable_interpolation_decimal",
            Value = "{value}",
            VariableValue = "1.234",
            Expected = "1.234"
        },
        new StringTestCase
        {
            Name = "variable_interpolation_string",
            Value = "{value}",
            VariableValue = "\"str\"",
            Expected = "str"
        },
        new StringTestCase
        {
            Name = "variable_interpolation_array",
            Value = "{value}",
            VariableValue = "[1,2,3]",
            Expected = "[1,2,3]"
        },
        new StringTestCase
        {
            Name = "variable_interpolation_object",
            Value = "{value}",
            VariableValue = "{key:\"value\"}",
            Expected = "{key:\"value\"}"
        }
    };
}