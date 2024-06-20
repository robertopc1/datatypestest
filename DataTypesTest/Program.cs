// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using System.Net.NetworkInformation;
using DataTypesTest;
using Microsoft.Extensions.Configuration;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;

public class Program
{
    public static void Main(string[] args)
    {
        //Grabbing appsettings.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();
        
        //Getting Connection string
        string connectionString = config.GetConnectionString("RedisConnectionString");

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException();
        
        var redisConnection = ConnectionMultiplexer.Connect(connectionString);
        
        IDatabase db = redisConnection.GetDatabase();
        SearchCommands ft = db.FT();

        string index = "idx-datatypes";

        ft.DropIndex(index);
        ft.Create(index, new FTCreateParams().On(IndexDataType.HASH)
                .Prefix("datatype:"),
            new Schema()
                .AddNumericField("doubleValue") 
                .AddNumericField("decimalValue"));
        
        string key = "datatype:example1";
        double doubleValue = 0.000014; //6740398.58;
        decimal decimalValue = Convert.ToDecimal(doubleValue);
        
        var dict = new Dictionary<string, dynamic>();

        //Try to use decimal for better precision. 
        dict.Add("doubleValue", doubleValue); 
        dict.Add("decimalValue", decimalValue);

        var he = Utilities.ConvertToRedisHashEntru(dict);
        
        //Store to Redis
        db.HashSet(key, he);

        //Retrieve with HashGetAll
        var result = db.HashGetAll(key);
        
        Debug.WriteLine($"Double Value: {result[0]}");
        Debug.WriteLine($"Decimal Value: {result[1]}");
        
        //Retrieve with Aggregate & Search
        var aggregateDoubleResult = ft.Aggregate(index,
            new AggregationRequest("*")
                .GroupBy("@doubleValue", Reducers.Avg("@doubleValue").As("avg")));
        
        var aggregateDecimalResult = ft.Aggregate(index,
            new AggregationRequest("*")
                .GroupBy("@decimalValue", Reducers.Avg("@decimalValue").As("avg")));

        var searchResult = ft.Search(index, new Query("*").Limit(0, 20));
    }
}





