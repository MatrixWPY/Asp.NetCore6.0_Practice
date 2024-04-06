using StackExchange.Redis;
using static Consumer_1.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string queueName = "StreamQueue";
string groupName = "QueueGroup";
string consumerName = "GroupConsumer_1";

#region 前置判斷
var hasKey = db.KeyExists(queueName, CommandFlags.None);
if (hasKey == false)
{
    Console.WriteLine($"Didn't found Stream Key.");
    Console.ReadKey();
    return;
}

var group = db.StreamGroupInfo(queueName);
if (group == null || group.Any(e => e.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase)) == false)
{
    Console.WriteLine($"Didn't found Group.");
    Console.ReadKey();
    return;
}
#endregion

Console.WriteLine("Consume from StreamQueue:");
while (true)
{
    var data = db.StreamReadGroup(queueName, groupName, consumerName, ">", count: 5, noAck: false);
    if (data.Any())
    {
        foreach (var item in data)
        {
            item.Values.ToList().ForEach(c =>
            {
                Console.WriteLine($"message-id:{item.Id} , {c.Name}:{c.Value}");
            });

            db.StreamAcknowledge(queueName, groupName, item.Id);
            db.StreamDelete(queueName, new[] { item.Id });
        }

        Console.WriteLine();
    }
}