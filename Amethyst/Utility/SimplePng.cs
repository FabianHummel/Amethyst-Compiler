using System.Text;
using Amethyst.Model;

namespace Amethyst.Utility;

public sealed class SimplePng
{
    private readonly Stream stream;
    private readonly int width, height;
    private readonly byte[] pixels; // RGBA, row-major

    public SimplePng(Stream stream, int width, int height)
    {
        this.stream = stream;
        this.width = width;
        this.height = height;
        pixels = new byte[width * height * 4];
    }

    public void Write(int x, int y, Color c)
    {
        int i = (y * width + x) * 4;
        pixels[i + 0] = c.R;
        pixels[i + 1] = c.G;
        pixels[i + 2] = c.B;
        pixels[i + 3] = c.A;
    }

    public void Save()
    {
        using var bw = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

        // PNG signature
        bw.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        WriteChunk(bw, "IHDR", BuildIHDR());
        WriteChunk(bw, "IDAT", BuildIDAT());
        WriteChunk(bw, "IEND", Array.Empty<byte>());
    }

    private byte[] BuildIHDR()
    {
        var d = new byte[13];
        WriteBE(d, 0, width);
        WriteBE(d, 4, height);
        d[8] = 8;  // bit depth
        d[9] = 6;  // RGBA
        d[10] = 0; // compression
        d[11] = 0; // filter
        d[12] = 0; // interlace
        return d;
    }

    private byte[] BuildIDAT()
    {
        using var ms = new MemoryStream();

        // zlib header: CMF/FLG (no compression)
        ms.WriteByte(0x78);
        ms.WriteByte(0x01);

        uint adlerA = 1, adlerB = 0;

        for (int y = 0; y < height; y++)
        {
            int rowLen = 1 + width * 4;
            bool final = (y == height - 1);

            ms.WriteByte((byte)(final ? 1 : 0)); // BFINAL + BTYPE=0
            ms.WriteByte((byte)(rowLen & 0xFF));
            ms.WriteByte((byte)(rowLen >> 8));
            ms.WriteByte((byte)(~rowLen & 0xFF));
            ms.WriteByte((byte)(~rowLen >> 8));

            ms.WriteByte(0); // filter 0
            UpdateAdler(ref adlerA, ref adlerB, 0);

            int rowStart = y * width * 4;
            for (int i = 0; i < width * 4; i++)
            {
                byte b = pixels[rowStart + i];
                ms.WriteByte(b);
                UpdateAdler(ref adlerA, ref adlerB, b);
            }
        }

        uint adler = (adlerB << 16) | adlerA;
        WriteBE(ms, adler);

        return ms.ToArray();
    }

    private static void WriteChunk(BinaryWriter bw, string type, byte[] data)
    {
        WriteBE(bw, (uint)data.Length);
        var t = Encoding.ASCII.GetBytes(type);
        bw.Write(t);
        bw.Write(data);
        WriteBE(bw, Crc32(t, data));
    }

    private static void WriteBE(byte[] b, int o, int v)
    {
        b[o + 0] = (byte)(v >> 24);
        b[o + 1] = (byte)(v >> 16);
        b[o + 2] = (byte)(v >> 8);
        b[o + 3] = (byte)v;
    }

    private static void WriteBE(BinaryWriter bw, uint v)
    {
        bw.Write((byte)(v >> 24));
        bw.Write((byte)(v >> 16));
        bw.Write((byte)(v >> 8));
        bw.Write((byte)v);
    }

    private static void WriteBE(Stream s, uint v)
    {
        s.WriteByte((byte)(v >> 24));
        s.WriteByte((byte)(v >> 16));
        s.WriteByte((byte)(v >> 8));
        s.WriteByte((byte)v);
    }

    private static void UpdateAdler(ref uint a, ref uint b, byte v)
    {
        a = (a + v) % 65521;
        b = (b + a) % 65521;
    }

    private static uint Crc32(byte[] type, byte[] data)
    {
        uint crc = type.Aggregate(0xFFFFFFFF, Step);
        crc = data.Aggregate(crc, Step);
        return crc ^ 0xFFFFFFFF;
    }

    private static uint Step(uint c, byte b)
    {
        c ^= b;
        for (int k = 0; k < 8; k++)
            c = (c & 1) != 0 ? (0xEDB88320 ^ (c >> 1)) : (c >> 1);
        return c;
    }
}