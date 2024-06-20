// See https://aka.ms/new-console-template for more information


using DataTypesTest;

var dict = new Dictionary<string, dynamic>();

dict.Add("test", 6740398.58);

var he = Utilities.ConvertToRedisHashEntru(dict);
