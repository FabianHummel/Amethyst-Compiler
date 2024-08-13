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
        
        AddCode($"scoreboard players operation {MemoryLocation} amethyst {op.ToSymbol()}= {rhs.ToNumber.Location} amethyst");

        return new IntResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString()
        };
    }

    protected override AbstractResult VisitAdd(IntResult rhs)
    {
        return Calculate(rhs, Operator.ADD);
    }

    protected override AbstractResult VisitAdd(DecResult rhs)
    {
        return Calculate(rhs, Operator.ADD);
    }

    protected override AbstractResult VisitAdd(BoolResult rhs)
    {
        return Calculate(rhs, Operator.ADD);
    }

    protected override AbstractResult VisitSubtract(IntResult rhs)
    {
        return Calculate(rhs, Operator.SUBTRACT);
    }

    protected override AbstractResult VisitSubtract(DecResult rhs)
    {
        return Calculate(rhs, Operator.SUBTRACT);
    }

    protected override AbstractResult VisitSubtract(BoolResult rhs)
    {
        return Calculate(rhs, Operator.SUBTRACT);
    }

    protected override AbstractResult VisitMultiply(IntResult rhs)
    {
        return Calculate(rhs, Operator.MULTIPLY);
    }

    protected override AbstractResult VisitMultiply(DecResult rhs)
    {
        return Calculate(rhs, Operator.MULTIPLY);
    }

    protected override AbstractResult VisitMultiply(BoolResult rhs)
    {
        return Calculate(rhs, Operator.MULTIPLY);
    }

    protected override AbstractResult VisitDivide(IntResult rhs)
    {
        return Calculate(rhs, Operator.DIVIDE);
    }

    protected override AbstractResult VisitDivide(DecResult rhs)
    {
        return Calculate(rhs, Operator.DIVIDE);
    }

    protected override AbstractResult VisitDivide(BoolResult rhs)
    {
        return Calculate(rhs, Operator.DIVIDE);
    }

    protected override AbstractResult VisitModulo(IntResult rhs)
    {
        return Calculate(rhs, Operator.MODULO);
    }

    protected override AbstractResult VisitModulo(DecResult rhs)
    {
        return Calculate(rhs, Operator.MODULO);
    }

    protected override AbstractResult VisitModulo(BoolResult rhs)
    {
        return Calculate(rhs, Operator.MODULO);
    }
}