using StackExchange.Redis;
using static Publish.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string streamKey = "StreamQueue";
string groupName = "QueueGroup";
string consumerName = "GroupConsumer_2";

var hasKey = db.KeyExists(streamKey, CommandFlags.None);
if (hasKey == false)
{
    Console.WriteLine($"Didn't found Stream Key.");
    Console.ReadKey();
    return;
}
var group = db.StreamGroupInfo(streamKey);
if (group == null || group.Any(e => e.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
{
    Console.WriteLine($"Didn't found Group.");
    Console.ReadKey();
    return;
}

Console.WriteLine("Consume from StreamQueue:");
while (true)
{
    var data = db.StreamReadGroup(streamKey, groupName, consumerName, ">", count: 5, noAck: false);
    if (data.Any())
    {
        foreach (var item in data)
        {
            item.Values.ToList().ForEach(c =>
            {
                Console.WriteLine($"message-id:{item.Id} , {c.Name}:{c.Value}");
            });

            db.StreamAcknowledge(streamKey, groupName, item.Id);
            db.StreamDelete(streamKey, new[] { item.Id });
        }

        Console.WriteLine();
    }
}