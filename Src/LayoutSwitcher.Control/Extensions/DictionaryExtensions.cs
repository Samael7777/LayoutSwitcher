namespace LayoutSwitcher.Control.Extensions;

internal static class DictionaryExtensions
{
    public static TValue AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue addValue,
        Func<TKey, TValue, TValue> updateValueFactory)
    where TKey : notnull
    {
        if (dict.TryGetValue(key, out var value))
        {
            var newValue = updateValueFactory(key, value);
            dict[key] = newValue;
            return newValue;
        }
        dict.Add(key, addValue);
        return addValue;
    }
}