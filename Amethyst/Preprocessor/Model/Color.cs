namespace Amethyst.Model;

public class Color
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public int Integer => (A << 24) | (R << 16) | (G << 8) | B;
    
    public string Hex => $"#{R:X2}{G:X2}{B:X2}{A:X2}";
}