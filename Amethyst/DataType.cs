namespace Amethyst;

public enum DataType
{
    Number,
    String,
    Boolean,
    Object,
    Array
}

public static class InferConstants
{
    public static Constant Calculate(Constant left, Constant right)
    {
        if (left.Type == DataType.String)
        {
            return new Constant
            {
                Type = DataType.String,
                Value = left.Value + right.Value
            };
        }

        if (left.Type == DataType.Number)
        {
            if (right.Type == DataType.Number)
            {
                return new Constant
                {
                    Type = DataType.Number,
                    Value = (int.Parse(left.Value) + int.Parse(right.Value)).ToString()
                };
            }

            return new Constant
            {
                Type = DataType.String,
                Value = left.Value + right.Value
            };
        }

        throw new NotImplementedException();
    }
}