namespace Tests.Utility;

public static class MathUtility
{
    public static int Modulo(int x, int y)
    {
        return (x % y + y) % y;
    }
}