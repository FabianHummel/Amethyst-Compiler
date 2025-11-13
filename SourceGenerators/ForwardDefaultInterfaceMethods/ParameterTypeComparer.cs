using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace SourceGenerators.ForwardDefaultInterfaceMethods;

internal sealed class ParameterTypeComparer : EqualityComparer<IParameterSymbol>
{
    public static readonly ParameterTypeComparer Instance = new();
    
    public override bool Equals(IParameterSymbol x, IParameterSymbol y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;
        return x.Type.Equals(y.Type, SymbolEqualityComparer.Default);
    }

    public override int GetHashCode(IParameterSymbol obj)
    {
        return SymbolEqualityComparer.Default.GetHashCode(obj.Type);
    }
}