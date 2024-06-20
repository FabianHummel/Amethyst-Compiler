namespace Amethyst.Model;

public class Return : Exception
{
    public Expr.Literal Value { get; }

    public Return(Expr.Literal value)
    {
        Value = value;
    }
}