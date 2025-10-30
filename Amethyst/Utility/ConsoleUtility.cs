using System.Diagnostics;
using static Crayon.Output;

namespace Amethyst.Utility;

/// <summary>Utility methods for console output with color and formatting.</summary>
public static class ConsoleUtility
{
    /// <summary><see cref="CancellationTokenSource" /> for managing long-running tasks in the console.
    /// This is used to track the time spent on long tasks and to allow for cancellation of ongoing tasks.</summary>
    private static CancellationTokenSource? LongTaskCts { get; set; }

    /// <summary>Indicates whether to use reduced colors in console output for better compatibility with
    /// different terminal types.</summary>
    public static bool IsReducedColors { get; set; }

    /// <summary>Clears the console screen and resets the cursor position.</summary>
    public static void ClearConsole()
    {
        Console.Clear();
        if (!IsReducedColors) Console.Write("\f\ec\e[3J");
        Console.SetCursorPosition(0, 1);
    }

    /// <summary>Clears the current console line. Used in loading animations, but ultimately disabled for
    /// better compatibility.</summary>
    public static void ClearCurrentConsoleLine()
    {
        // int currentLineCursor = Console.CursorTop;
        // Console.SetCursorPosition(0, Console.CursorTop);
        // Console.Write(new string(' ', Console.WindowWidth)); 
        // Console.SetCursorPosition(0, currentLineCursor);
    }

    /// <summary>Starts a long task indicator in the console with a loading animation.</summary>
    /// <param name="executionText">The text to display while the long task is running.</param>
    /// <param name="getElapsed">A function that returns the elapsed time in milliseconds since the task
    /// started.</param>
    /// <returns>A <see cref="CancellationTokenSource" /> that can be used to cancel the long task
    /// indicator.</returns>
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

    /// <summary>Prints a message to the console with the elapsed time in milliseconds.</summary>
    /// <param name="s">The message to print.</param>
    /// <param name="elapsed">The elapsed time in milliseconds.</param>
    public static void PrintMessageWithTime(string s, long elapsed)
    {
        ClearCurrentConsoleLine();
        Console.WriteLine(UseColorOption(
            $"\r {Dim("\u279c")} {s} {Dim($"({elapsed}ms)")}",
            $"\r \u279c {s} ({elapsed}ms)")
            .PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    /// <summary>Prints a debug message to the console with the elapsed time in milliseconds.</summary>
    /// <param name="s">The message to print.</param>
    /// <param name="elapsed">The elapsed time in milliseconds.</param>
    public static void PrintDebugMessageWithTime(string s, long elapsed)
    {
        ClearCurrentConsoleLine();
        Console.WriteLine(UseColorOption(
            $"\r {Dim("\u279c")} {Dim(s)} {Dim($"({elapsed}ms)")}",
            $"\r \u279c {s} ({elapsed}ms)")
            .PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    /// <summary>Prints an error to the console.</summary>
    /// <param name="s">The error to print.</param>
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

    /// <summary>Prints a warning to the console.</summary>
    /// <param name="s">The warning to print.</param>
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
    
    /// <summary>Prints a debug message to the console.</summary>
    /// <param name="s">The debug message to print.</param>
    public static void PrintDebug(string s)
    {
        ClearCurrentConsoleLine();
        if (IsReducedColors) Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(UseColorOption(
            $"\r {Dim("\u279c")} {Dim(s)}",
            $"\r \u279c {s}").PadRight(Console.WindowWidth));
        Console.ResetColor();
    }
    
    /// <summary>Selects between two string options based on the IsReducedColors setting.</summary>
    /// <param name="withColor">The string to use when colors are enabled.</param>
    /// <param name="withoutColor">The string to use when colors are reduced.</param>
    /// <returns>The appropriate string based on the IsReducedColors setting.</returns>
    private static string UseColorOption(string withColor, string withoutColor)
    {
        return IsReducedColors ? withoutColor : withColor;
    }
}