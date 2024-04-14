namespace Amethyst;

public static class DynamicArithmetics
{
    public static dynamic Add(dynamic a, dynamic b)
    {
        return a + b;
    }

    public static dynamic Sub(dynamic a, dynamic b)
    {
        return a - b;
    }

    public static dynamic Mul(dynamic a, dynamic b)
    {
        return a * b;
    }

    public static dynamic Div(dynamic a, dynamic b)
    {
        return a / b;
    }

    public static dynamic Mod(dynamic a, dynamic b)
    {
        return a % b;
    }

    public static dynamic Gt(dynamic a, dynamic b)
    {
        return a > b;
    }

    public static dynamic Ge(dynamic a, dynamic b)
    {
        return a >= b;
    }

    public static dynamic Lt(dynamic a, dynamic b)
    {
        return a < b;
    }

    public static dynamic Le(dynamic a, dynamic b)
    {
        return a <= b;
    }

    public static dynamic Ne(dynamic a, dynamic b)
    {
        return !a.Equals(b);
    }

    public static dynamic Eq(dynamic a, dynamic b)
    {
        return a.Equals(b);
    }
    
    public static dynamic And(dynamic a, dynamic b)
    {
        return a && b;
    }
    
    public static dynamic Or(dynamic a, dynamic b)
    {
        return a || b;
    }
}