using StackExchange.Redis;
using static Publish.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string streamKey = "StreamQueue";
string groupName = "QueueGroup";

var hasKey = db.KeyExists(streamKey, CommandFlags.None);
if (hasKey == false)
{
    db.StreamAdd(streamKey, "-1", "QueueRoot");
}
var groups = db.StreamGroupInfo(streamKey, CommandFlags.None);
if (groups == null || groups?.Any(x => x.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
{
    try
    {
        db.StreamCreateConsumerGroup(streamKey, groupName, StreamPosition.NewMessages);
    }
    catch
    {
        Console.WriteLine($"Create Consumer Group Fail.");
        Console.ReadKey();
        throw;
    }
}

Console.WriteLine("Produce to StreamQueue:");
for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"{i + 1}:Test_{i + 1}");
    db.StreamAdd(streamKey, $"{i + 1}", $"Test_{i + 1}");
}

Console.WriteLine("Produce Finish");
Console.ReadKey();