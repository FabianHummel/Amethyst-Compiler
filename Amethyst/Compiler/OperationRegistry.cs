using System.Diagnostics.CodeAnalysis;
using Amethyst.Model;
using Amethyst.Utility;
using Antlr4.Runtime;

namespace Amethyst;

using OperationImplementation = Func<Compiler, ParserRuleContext, AbstractValue, AbstractValue, AbstractValue>;

public static class OperationRegistry
{
    private class LhsDict : Dictionary<Type, RhsDict>;

    private class RhsDict : Dictionary<Type, OperationImplementation>;
    
    private static readonly Dictionary<Enum, LhsDict> _operations = new();
    
    public static void Register<TLhs, TRhs, TRet>(Enum op, Func<Compiler, ParserRuleContext, TLhs, TRhs, TRet> impl)
        where TLhs : AbstractValue
        where TRhs : AbstractValue
        where TRet : AbstractValue
    {
        if (!_operations.TryGetValue(op, out var lhsDict))
        {
            lhsDict = new LhsDict();
            _operations.Add(op, lhsDict);
        }

        if (!lhsDict.TryGetValue(typeof(TLhs), out var rhsDict))
        {
            rhsDict = new RhsDict();
            lhsDict.Add(typeof(TLhs), rhsDict);
        }

        if (!rhsDict.TryAdd(typeof(TRet), WrappedImpl))
        {
            throw new InvalidOperationException($"Operation already registered for {typeof(TLhs).Name} {op} {typeof(TRhs).Name}");
        }

        return;

        AbstractValue WrappedImpl(Compiler compiler, ParserRuleContext context, AbstractValue lhs, AbstractValue rhs)
        {
            return impl(compiler, context, (TLhs)lhs, (TRhs)rhs);
        }
    }

    public static void Register<TLhs, TRhs, TRet, TOp>(Func<Compiler, ParserRuleContext, TLhs, TRhs, TOp, TRet> impl)
        where TLhs : AbstractValue
        where TRhs : AbstractValue
        where TRet : AbstractValue
        where TOp : struct, Enum
    {
        foreach (var arithmeticOperator in Enum.GetValues<TOp>())
        {
            Register<TLhs, TRhs, TRet>(arithmeticOperator, (compiler, context, lhs, rhs) =>
            {
                return impl(compiler, context, lhs, rhs, arithmeticOperator);
            });
        }
    }

    public static bool Resolve<TRet, TOp>(Compiler compiler, ParserRuleContext context, TOp op, AbstractValue lhs, AbstractValue rhs, [NotNullWhen(true)] out TRet? result)
        where TRet : AbstractValue
        where TOp : struct, Enum
    {
        result = null;
        
        if (!_operations.TryGetValue(op, out var lhsDict))
        {
            return false;
        }
        
        if (!TrySearchMatchingImplementation(lhsDict, lhs.GetType(), rhs.GetType(), out var impl))
        {
            return false;
        }
        
        result = (TRet)impl(compiler, context, lhs, rhs);
        return true;
    }
    
    public static TRet Resolve<TRet, TOp>(Compiler compiler, ParserRuleContext context, TOp op, AbstractValue lhs, AbstractValue rhs)
        where TRet : AbstractValue
        where TOp : struct, Enum
    {
        if (Resolve<TRet, TOp>(compiler, context, op, lhs, rhs, out var result))
        {
            return result;
        }

        throw new InvalidOperationException($"No operation registered for {lhs.Datatype} {op.GetAmethystOperatorSymbol()} {rhs.Datatype}");
    }

    private static bool TrySearchMatchingImplementation(LhsDict dictionary, Type lhs, Type rhs, [NotNullWhen(true)] out OperationImplementation? value)
    {
        value = null;
        
        if (dictionary.TryGetValue(lhs, out var rhsDict) && TrySearchMatchingImplementation(rhsDict, rhs, out value))
        {
            return true;
        }
        
        if (lhs.BaseType == null)
        {
            return false;
        }
        
        return TrySearchMatchingImplementation(dictionary, lhs.BaseType, rhs, out value);
    }

    private static bool TrySearchMatchingImplementation(RhsDict dictionary, Type rhs, [NotNullWhen(true)] out OperationImplementation? value)
    {
        value = null;
        
        if (dictionary.TryGetValue(rhs, out value))
        {
            return true;
        }
        
        if (rhs.BaseType == null)
        {
            return false;
        }
        
        return TrySearchMatchingImplementation(dictionary, rhs.BaseType, out value);
    }
}