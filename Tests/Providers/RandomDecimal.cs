using System.Collections;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Tests.Providers;

[AttributeUsage(AttributeTargets.Parameter)]
public class RandomDecimalAttribute : NUnitAttribute, IParameterDataSource
{
    private readonly int _digits;
    private readonly RandomAttribute _randomAttribute;

    public RandomDecimalAttribute(double min, double max, int count, int digits)
    {
        _digits = digits;
        _randomAttribute = new RandomAttribute(min, max, count);
    }
    
    public IEnumerable GetData(IParameterInfo parameter)
    {
        return _randomAttribute.GetData(parameter)
            .Cast<double>()
            .Select(value => Math.Round(value, _digits));
    }
}