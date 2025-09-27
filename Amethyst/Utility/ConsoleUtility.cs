using System.Diagnostics;
using Amethyst.Model;
using static Amethyst.Constants;
using static Crayon.Output;

namespace Amethyst.Utility;

public static class ConsoleUtility
{
    private static CancellationTokenSource? LongTaskCts { get; set; }

    public static bool IsReducedColors { get; set; }

    public static void ClearConsole()
    {
        Console.Clear();
        if (!IsReducedColors) Console.Write("\f\ec\e[3J");
        Console.SetCursorPosition(0, 1);
    }
    
    public static void ClearCurrentConsoleLine()
    {
        // int currentLineCursor = Console.CursorTop;
        // Console.SetCursorPosition(0, Console.CursorTop);
        // Console.Write(new string(' ', Console.WindowWidth)); 
        // Console.SetCursorPosition(0, currentLineCursor);
    }

    public static CancellationTokenSource PrintLongTask(string executionText, out Func<long> getElapsed)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var cts = LongTaskCts = new CancellationTokenSource();
        getElapsed = () => stopwatch.ElapsedMilliseconds;
        Task.Run(() => {
            var i = 0;
            while (!cts.Token.IsCancellationRequested)
            {
                ClearCurrentConsoleLine();
                var loadingSymbol = "\u280b\u2819\u2839\u2838\u283c\u2834\u2826\u2827\u2807\u280f"[i];
                Console.Write(UseColorOption(
                    $"\r {Dim($"\u279c {executionText} {loadingSymbol} ")}"
                    , $"\r \u279c {executionText} {loadingSymbol} ")
                    .PadRight(Console.WindowWidth));
                i = (i + 1) % 10;
                Console.ResetColor();
                Thread.Sleep(100);
            }

            stopwatch.Stop(); 
        }, cts.Token);
        return cts;
    }
    
    public static void PrintMessageWithTime(string s, long elapsed)
    {
        ClearCurrentConsoleLine();
        Console.WriteLine(UseColorOption(
            $"\r {Dim("\u279c")} {s} {Dim($"({elapsed}ms)")}",
            $"\r \u279c {s} ({elapsed}ms)")
            .PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    public static void PrintDebugMessageWithTime(string s, long elapsed)
    {
        ClearCurrentConsoleLine();
        Console.WriteLine(UseColorOption(
            $"\r {Dim("\u279c")} {Dim(s)} {Dim($"({elapsed}ms)")}",
            $"\r \u279c {s} ({elapsed}ms)")
            .PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    public static void PrintError(string s)
    {
        LongTaskCts?.Cancel();
        ClearCurrentConsoleLine();
        if (IsReducedColors) Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(UseColorOption(
            $"\r {Red().Dim("\u279c")} {Red().Bold(s)}",
            $"\r \u279c {s}")
            .PadRight(Console.WindowWidth));
        Console.ResetColor();
    }

    public static void PrintWarning(string s)
    {
        ClearCurrentConsoleLine();
        if (IsReducedColors) Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(UseColorOption(
            $"\r {Yellow().Dim("\u279c")} {s}",
            $"\r \u279c {s}")
            .PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    public static void PrintDebug(string s)
    {
        ClearCurrentConsoleLine();
        if (IsReducedColors) Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(UseColorOption(
            $"\r {Dim("\u279c")} {Dim(s)}",
            $"\r \u279c {s}").PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    private static string UseColorOption(string withColor, string withoutColor)
    {
        return IsReducedColors ? withoutColor : withColor;
    }
}

public static class AmethystExtensions
{
    public static void PrintAmethystLogoAndVersion(this Processor amethyst)
    {
        if (ConsoleUtility.IsReducedColors)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.Write(" Amethyst ");
        }
        else
        {
            Console.Write(" \e[1m");
            Console.Write($"\e[38;5;{98}mA");
            Console.Write($"\e[38;5;{98}mm");
            Console.Write($"\e[38;5;{98}me");
            Console.Write($"\e[38;5;{140}mt");
            Console.Write($"\e[38;5;{140}mh");
            Console.Write($"\e[38;5;{183}my");
            Console.Write($"\e[38;5;{183}ms");
            Console.Write($"\e[38;5;{183}mt");
            Console.Write("\e[22m ");
        }
        
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