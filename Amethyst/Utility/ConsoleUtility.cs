using System.Diagnostics;
using Amethyst.Model;
using static Amethyst.Constants;
using static Crayon.Output;

namespace Amethyst.Utility;

public static class ConsoleUtility
{
    private static CancellationTokenSource? LongTaskCts { get; set; }

    public static void ClearConsole()
    {
        Console.Clear();
        Console.Write("\f\u001bc\x1b[3J");
        Console.SetCursorPosition(0, 1);
    }

    public static CancellationTokenSource PrintLongTask(string executionText, out Func<long> getElapsed)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var cts = LongTaskCts = new CancellationTokenSource();
        getElapsed = () => stopwatch.ElapsedMilliseconds;
        Task.Run(() =>
            {
                var i = 0;
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.Write($"\r {Dim($"\u279c {executionText} {"\u280b\u2819\u2839\u2838\u283c\u2834\u2826\u2827\u2807\u280f"[i]} ")}".PadRight(Console.WindowWidth));
                    i = (i + 1) % 10;
                    Console.ResetColor();
                    Thread.Sleep(100);
                }

                stopwatch.Stop();
            },
            cts.Token);
        return cts;
    }
    
    public static void PrintMessageWithTime(string s, long elapsed)
    {
        Console.WriteLine($"\r {Dim("\u279c")} {s} {Dim($"({elapsed}ms)")}".PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    public static void PrintDebugMessageWithTime(string s, long elapsed)
    {
        Console.WriteLine($"\r {Dim("\u279c")} {Dim(s)} {Dim($"({elapsed}ms)")}".PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    public static void PrintError(string s)
    {
        LongTaskCts?.Cancel();
        Console.WriteLine($"\r {Red().Dim("\u279c")} {Red().Bold(s)}".PadRight(Console.WindowWidth));
        Console.ResetColor();
    }

    public static void PrintWarning(string s)
    {
        Console.WriteLine($"\r {Yellow().Dim("\u279c")} {s}".PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    public static void PrintDebug(string s)
    {
        Console.WriteLine($"\r {Dim("\u279c")} {Dim(s)}".PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
}

public static class AmethystExtensions
{
    public static void PrintAmethystLogoAndVersion(this Processor amethyst)
    {
        Console.Write(" \x1b[1m");
        Console.Write($"\x1b[38;5;{98}mA");
        Console.Write($"\x1b[38;5;{98}mm");
        Console.Write($"\x1b[38;5;{98}me");
        Console.Write($"\x1b[38;5;{140}mt");
        Console.Write($"\x1b[38;5;{140}mh");
        Console.Write($"\x1b[38;5;{183}my");
        Console.Write($"\x1b[38;5;{183}ms");
        Console.Write($"\x1b[38;5;{183}mt");
        Console.Write("\x1b[22m ");
        
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write("v" + AMETHYST_VERSION);
        if (amethyst.Context.CompilerFlags.HasFlag(CompilerFlags.Watch))
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write(" (watch mode)");
        }
        if (amethyst.Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(" (debug mode)");
        }
        Console.WriteLine();
        Console.ResetColor();
        Console.WriteLine();
    }
}