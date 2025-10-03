using Amethyst;
using NUnit.Framework.Interfaces;

namespace Tests;

public partial class TestMain
{
    internal static void CreateDatapackForTest(ITest test)
    {
        if (test.Method is not { } methodInfo)
        {
            return;
        }
        
        if (methodInfo.GetCustomAttributes<AmethystProjectAttribute>(false).First() is not { } amethystProject)
        {
            return;
        }

        var rootDir = Path.Combine(Environment.CurrentDirectory, amethystProject.Path);
        _amethyst = new Processor(rootDir, 0, overrideMinecraftRoot: Environment.CurrentDirectory);
    }
}