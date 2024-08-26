using Amethyst.Model;

namespace Amethyst;

public abstract class NumericBase : AbstractResult
{
    public abstract override DataType DataType { get; }

    private AbstractResult Calculate(AbstractResult rhs, Operator op)
    {
        AddCode($"scoreboard players operation {MemoryLocation} amethyst = {Location} amethyst");

        if (rhs.DataType.Scale != 1)
        {
            AddCode($"scoreboard players operation {MemoryLocation} amethyst *= .{rhs.DataType.Scale} amethyst_const");
        }
        
        AddCode($"scoreboard players operation {MemoryLocation} amethyst {op.ToSymbol()}= {rhs.MakeNumber().Location} amethyst");

        return new IntegerResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString()
        };
    }

    protected override AbstractResult VisitAdd(IntegerResult rhs)
    {
        return Calculate(rhs, Operator.ADD);
    }

    protected override AbstractResult VisitAdd(DecimalResult rhs)
    {
        return Calculate(rhs, Operator.ADD);
    }

    protected override AbstractResult VisitAdd(BooleanResult rhs)
    {
        return Calculate(rhs, Operator.ADD);
    }

    protected override AbstractResult VisitSubtract(IntegerResult rhs)
    {
        return Calculate(rhs, Operator.SUBTRACT);
    }

    protected override AbstractResult VisitSubtract(DecimalResult rhs)
    {
        return Calculate(rhs, Operator.SUBTRACT);
    }

    protected override AbstractResult VisitSubtract(BooleanResult rhs)
    {
        return Calculate(rhs, Operator.SUBTRACT);
    }

    protected override AbstractResult VisitMultiply(IntegerResult rhs)
    {
        return Calculate(rhs, Operator.MULTIPLY);
    }

    protected override AbstractResult VisitMultiply(DecimalResult rhs)
    {
        return Calculate(rhs, Operator.MULTIPLY);
    }

    protected override AbstractResult VisitMultiply(BooleanResult rhs)
    {
        return Calculate(rhs, Operator.MULTIPLY);
    }

    protected override AbstractResult VisitDivide(IntegerResult rhs)
    {
        return Calculate(rhs, Operator.DIVIDE);
    }

    protected override AbstractResult VisitDivide(DecimalResult rhs)
    {
        return Calculate(rhs, Operator.DIVIDE);
    }

    protected override AbstractResult VisitDivide(BooleanResult rhs)
    {
        return Calculate(rhs, Operator.DIVIDE);
    }

    protected override AbstractResult VisitModulo(IntegerResult rhs)
    {
        return Calculate(rhs, Operator.MODULO);
    }

    protected override AbstractResult VisitModulo(DecimalResult rhs)
    {
        return Calculate(rhs, Operator.MODULO);
    }

    protected override AbstractResult VisitModulo(BooleanResult rhs)
    {
        return Calculate(rhs, Operator.MODULO);
    }
}