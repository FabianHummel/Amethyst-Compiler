using Amethyst.Model;

namespace Amethyst;

public abstract partial class AbstractNumericPreprocessorValue : AbstractPreprocessorValue
{
    private AbstractNumericPreprocessorValue Calculate(AbstractNumericPreprocessorValue lhs, AbstractNumericPreprocessorValue rhs, ArithmeticOperator op)
    {
        throw new NotImplementedException();
    }
    
    private AbstractNumericPreprocessorValue Calculate(AbstractNumericPreprocessorValue lhs, AbstractNumericPreprocessorValue rhs, ComparisonOperator op)
    {
        throw new NotImplementedException();
    }
}