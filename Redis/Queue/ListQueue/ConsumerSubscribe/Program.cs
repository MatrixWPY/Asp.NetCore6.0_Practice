using static ConsumerSubscribe.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string channelName = "ListChannel";

Console.WriteLine($"Subscribe from {channelName}");
var sub = redis.GetSubscriber();
sub.Subscribe(channelName, (chl, msg) =>
{
    string queueName = msg;
    Console.WriteLine($"Consume from {queueName}:");

    while (db.ListLength(queueName) > 0)
    {
        var data = db.ListRightPop(queueName);
        if (data.IsNullOrEmpty)
        {
            break;
        }
        Console.WriteLine(data);
    }

    Console.WriteLine("Consume Finish");
});

Console.ReadKey();