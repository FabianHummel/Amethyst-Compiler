using Amethyst.Model;

namespace Amethyst;

public class PreprocessorDecimalResult : PreprocessorResult<double>
{
    public override PreprocessorDataType DataType => new()
    {
        BasicType = BasicPreprocessorType.Dec
    };

    public override bool AsBoolean => Value != 0.0d;
    
    public override int AsInteger => (int)Value;

    public override string ToString()
    {
        return Value.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
    }
}