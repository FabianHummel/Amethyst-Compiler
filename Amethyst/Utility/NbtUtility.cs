using Amethyst.Model;

namespace Amethyst.Utility;

public static class NbtUtility
{
    public static string ToNbtString(this object obj)
    {
        return obj switch
        {
            string value => $"\"{value}\"", // TODO: Escape quotes and other special characters in the string that need escaping in Minecraft
            bool value => value ? "1b" : "0b",
            object[] value => $"[{string.Join(',', value.Select(o => $"{{_:{o.ToNbtString()}}}"))}]",
            _ => $"{obj}"
        };
    }
    
    public static object ParseScoreboardValue(int value, AbstractDatatype variableAbstractDatatype)
    {
        if (variableAbstractDatatype.DataLocation != DataLocation.Scoreboard)
        {
            throw new InvalidOperationException($"Invalid data type '{variableAbstractDatatype}' for scoreboard variable.");
        }

        if (variableAbstractDatatype is DecimalDatatype decimalDatatype)
        {
            var scale = (int)Math.Pow(10, decimalDatatype.DecimalPlaces);
            return (double)value / scale;
        }

        return variableAbstractDatatype switch
        {
            IntegerDatatype => value,
            BooleanDatatype => value != 0,
            RawDatatype => value,
            _ => throw new InvalidOperationException($"Invalid data type '{variableAbstractDatatype}' for scoreboard variable.")
        };
    }
    
    public static object ParseStorageValue(string value, AbstractDatatype variableAbstractDatatype)
    {
        if (variableAbstractDatatype.DataLocation != DataLocation.Storage)
        {
            throw new InvalidOperationException($"Invalid data type '{variableAbstractDatatype}' for storage variable.");
        }
        
        if (variableAbstractDatatype.Modifier == Modifier.Array || variableAbstractDatatype is ArrayDatatype)
        {
            var elements = value[1..^1].Split(',').Select(element =>
            {
                var variableDatatype = ParseStorageType(element);
                return ParseStorageValue(element.Trim(), variableDatatype);
            });
            
            return elements.ToArray();
        }

        if (variableAbstractDatatype.Modifier == Modifier.Object || variableAbstractDatatype is ObjectDatatype)
        {
            var keyValuePairs = value[1..^1].Split(',').Select(pair =>
            {
                var parts = pair.Split(':', 2);
                var key = parts[0].Trim();
                var val = parts[1].Trim();
                var variableDatatype = ParseStorageType(val);
                return new KeyValuePair<string, object>(key, ParseStorageValue(val, variableDatatype));
            });
            
            return keyValuePairs.ToDictionary();
        }
        
        return variableAbstractDatatype.BasicType switch
        {
            BasicType.Int => int.Parse(value),
            BasicType.Bool => value.EndsWith('b') && value != "0b",
            BasicType.Dec => double.Parse(value.TrimEnd('d')),
            BasicType.String => value[1..^1],
            BasicType.Entity => Guid.Parse(value[1..^1]),
            BasicType.Raw => value,
            _ => throw new InvalidOperationException($"Invalid data type '{variableAbstractDatatype}' for storage variable.")
        };
    }

    private static AbstractDatatype ParseStorageType(string element)
    {
        if (element.StartsWith('[') && element.EndsWith(']'))
        {
            return new ArrayDatatype();
        }
        
        if (element.StartsWith('{') && element.EndsWith('}'))
        {
            return new ObjectDatatype();
        }
        
        if (element.StartsWith('"') && element.EndsWith('"'))
        {
            if (element[1..^1].Length == 36 && Guid.TryParse(element[1..^1], out _))
            {
                return new EntityDatatype();
            }

            return new StringDatatype();
        }
        
        if (element.EndsWith('b'))
        {
            return new BooleanDatatype();
        }
        
        if (element.EndsWith('d'))
        {
            var decimalPart = element.Split('.')[1].TrimEnd('d');
            int decimalPlaces;
            if (decimalPart == "0")
            {
                decimalPlaces = 0;
            }
            else
            {
                decimalPlaces = decimalPart.Length;
            }

            return new DecimalDatatype(decimalPlaces);
        }

        throw new InvalidOperationException($"Could not parse '{element}' as storage value.");
    }
}