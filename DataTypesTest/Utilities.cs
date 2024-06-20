using StackExchange.Redis;

namespace DataTypesTest;

public static class Utilities
{
    
    public static HashEntry[] ConvertToRedisHashEntru(Dictionary<string, dynamic> dictDocRedisDynamic)
    {
        var hashEntries = new HashEntry[dictDocRedisDynamic.Count];
        int index = 0;

        foreach (var item in dictDocRedisDynamic)
        {
            if (item.Value is decimal || item.Value is float)
                hashEntries[index++] = new HashEntry(item.Key, item.Value.Tostring());
        }

        return hashEntries;
    }
}