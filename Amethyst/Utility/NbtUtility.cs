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
    
    public static object ParseScoreboardValue(int value, DataType variableDataType)
    {
        if (variableDataType.Modifier != null)
        {
            throw new InvalidOperationException($"Invalid data type '{variableDataType}' for scoreboard variable.");
        }

        if (variableDataType is DecimalDataType decimalDataType)
        {
            var scale = (int)Math.Pow(10, decimalDataType.DecimalPlaces);
            return (double)value / scale;
        }

        return variableDataType.BasicType switch
        {
            BasicType.Int => value,
            BasicType.Bool => value != 0,
            _ => throw new InvalidOperationException($"Invalid data type '{variableDataType}' for scoreboard variable.")
        };
    }
    
    public static object ParseStorageValue(string value, DataType variableDataType)
    {
        if (variableDataType.Modifier == Modifier.Array || variableDataType.BasicType == BasicType.Array)
        {
            var elements = value[1..^1].Split(',').Select(element =>
            {
                var variableDataType = ParseStorageType(element);
                return ParseStorageValue(element.Trim(), variableDataType);
            });
            
            return elements.ToArray();
        }

        if (variableDataType.Modifier == Modifier.Object || variableDataType.BasicType == BasicType.Object)
        {
            var keyValuePairs = value[1..^1].Split(',').Select(pair =>
            {
                var parts = pair.Split(':', 2);
                var key = parts[0].Trim();
                var val = parts[1].Trim();
                var variableDataType = ParseStorageType(val);
                return new KeyValuePair<string, object>(key, ParseStorageValue(val, variableDataType));
            });
            
            return keyValuePairs.ToDictionary();
        }
        
        return variableDataType.BasicType switch
        {
            BasicType.Int => int.Parse(value),
            BasicType.Bool => value.EndsWith('b') && value != "0b",
            BasicType.Dec => double.Parse(value.TrimEnd('d')),
            BasicType.String => value[1..^1],
            BasicType.Entity => Guid.Parse(value[1..^1]),
            _ => throw new InvalidOperationException($"Invalid data type '{variableDataType}' for storage variable.")
        };
    }

    private static DataType ParseStorageType(string element)
    {
        if (element.StartsWith('[') && element.EndsWith(']'))
        {
            return new DataType
            {
                BasicType = BasicType.Array
            };
        }
        
        if (element.StartsWith('{') && element.EndsWith('}'))
        {
            return new DataType
            {
                BasicType = BasicType.Object
            };
        }
        
        if (element.StartsWith('"') && element.EndsWith('"'))
        {
            if (element[1..^1].Length == 36 && Guid.TryParse(element[1..^1], out _))
            {
                return new DataType
                {
                    BasicType = BasicType.Entity
                };
            }
            
            return new DataType
            {
                BasicType = BasicType.String
            };
        }
        
        if (element.EndsWith('b'))
        {
            return new DataType
            {
                BasicType = BasicType.Bool
            };
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
            
            return new DecimalDataType
            {
                BasicType = BasicType.Dec,
                DecimalPlaces = decimalPlaces
            };
        }

        return new DataType
        {
            BasicType = BasicType.Unknown
        };
    }
}