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
            if(item.Value is decimal)
                hashEntries[index++] = new HashEntry(item.Key, item.Value.ToString()); // this will store exaxct
            
            if(item.Value is double)
                hashEntries[index++] = new HashEntry(item.Key, item.Value); // This will store with extra digitis added
        }

        return hashEntries;
    }
}