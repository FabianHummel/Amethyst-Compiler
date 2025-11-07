using JetBrains.Annotations;
using Tomlet.Attributes;
using static Amethyst.Model.Constants;

namespace Amethyst.Model;

public class Datapack
{
    /// <summary>The name of the datapack.</summary>
    [TomlProperty("name")]
    public required string Name { get; [UsedImplicitly] init; } = DefaultDatapackName;

    /// <summary>The output directory of the resulting datapack. Already includes the datapack name.</summary>
    [TomlProperty("output")]
    public required string? Output { get; [UsedImplicitly] init; }

    /// <summary>The description of the datapack.</summary>
    [TomlProperty("description")]
    public required string? Description { get; [UsedImplicitly] init; } = DefaultDatapackDescription;

    /// <summary>The format version of the datapack.</summary>
    [TomlProperty("pack_format")]
    public required int? PackFormat { get; [UsedImplicitly] init; } = DefaultDatapackFormat;

    /// <summary>The absolute path to the output directory where files will be generated into.</summary>
    [TomlNonSerialized]
    public string OutputDir { get; set; } = null!;

    /// <summary>List of MCF function paths to be executed every tick.</summary>
    [TomlNonSerialized]
    public HashSet<string> TickFunctions { get; } = new();

    /// <summary>List of MCF function paths to be executed on load.</summary>
    [TomlNonSerialized]
    public SortedSet<string> LoadFunctions { get; } = new(new LoadFunctionsComparer());
    
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public Datapack()
    {
    }
    
    public class LoadFunctionsComparer : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var xParts = x.Split(':');
            var yParts = y.Split(':');
            var xNs = xParts[0];
            var yNs = yParts[0];
            var xFn = xParts.Length > 1 ? xParts[1].Split('/').LastOrDefault() : "";
            var yFn = yParts.Length > 1 ? yParts[1].Split('/').LastOrDefault() : "";

            int nsCompare =
                xNs == InternalNamespaceName && yNs != InternalNamespaceName ? -1 :
                yNs == InternalNamespaceName && xNs != InternalNamespaceName ? 1 :
                string.Compare(xNs, yNs, StringComparison.Ordinal);
            if (nsCompare != 0) return nsCompare;

            int fnCompare =
                xFn == InitFunctionName && yFn != InitFunctionName ? -1 :
                yFn == InitFunctionName && xFn != InitFunctionName ? 1 :
                string.Compare(xFn, yFn, StringComparison.Ordinal);
            if (fnCompare == 0) return 1;
            return fnCompare;
        }
    }
}