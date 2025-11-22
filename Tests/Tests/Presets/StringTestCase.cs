using JetBrains.Annotations;

namespace Tests.Presets;

public class StringTestCase(string name, string value, string? variableValue = null)
{
    [UsedImplicitly]
    public string Name { get; } = name;

    [UsedImplicitly]
    public string Value { get; } = value;

    [UsedImplicitly]
    public string VariableValue { get; } = variableValue;

    public override string ToString() => Name;
    
    public static readonly IEnumerable<StringTestCase> Regular = new[]
    {
        new StringTestCase("basic", "Basic String"),
        new StringTestCase("special_characters", "!$%&/(=?+*#'"),
        new StringTestCase("escaping", "\\ \" \{ \'"),
        new StringTestCase("unicode", "\u1234"),
        new StringTestCase("empty", "")
        new StringTestCase("whitespace", "    ")
    };

    public static readonly IEnumerable<StringTestCase> Interpolated = new[]
    {
        new StringTestCase("regular", "Regular String"),
        new StringTestCase("basic_interpolation", "{123}"),
        new StringTestCase("nested_interpolation", "Hello {\"World\"}"),
        new StringTestCase("variable_interpolation_integer", "{value}", "123"),
        new StringTestCase("variable_interpolation_string", "{value}", "\"str\""),
        new StringTestCase("variable_interpolation_array", "{value}", "[1,2,3]"),
        new StringTestCase("variable_interpolation_object", "{value}", "{key:\"value\"}"),
    };
}