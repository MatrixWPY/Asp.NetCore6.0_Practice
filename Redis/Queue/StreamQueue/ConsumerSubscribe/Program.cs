using StackExchange.Redis;
using System.Diagnostics;
using static ConsumerSubscribe.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string channelName = "StreamChannel";
string consumerName = $"GroupConsumer_{Process.GetCurrentProcess().Id}";

Console.WriteLine($"Subscribe from {channelName}");
var sub = redis.GetSubscriber();
sub.Subscribe(channelName, (chl, msg) =>
{
    string queueName = msg.ToString().Split(':')[0];
    string groupName = msg.ToString().Split(':')[1];

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

    Console.WriteLine($"Consume from {queueName}:");
    var data = db.StreamReadGroup(queueName, groupName, consumerName, ">", count: (int)db.StreamLength(queueName), noAck: false);
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
    }
    Console.WriteLine("Consume Finish");
});

Console.ReadKey();