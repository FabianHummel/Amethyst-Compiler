namespace Amethyst.Utility;

public class KeyValuePairClass<TKey, TValue>
{
    private readonly KeyValuePair<TKey, TValue> _kvp;
    
    public KeyValuePairClass(TKey key, TValue value)
    {
        _kvp = new KeyValuePair<TKey, TValue>(key, value);
    }
    
    public TKey Key => _kvp.Key;
    public TValue Value => _kvp.Value;
}