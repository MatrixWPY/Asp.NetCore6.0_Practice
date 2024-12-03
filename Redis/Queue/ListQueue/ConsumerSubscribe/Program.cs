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

    var data = db.ListRightPop(queueName, count: db.ListLength(queueName));
    if (data?.Any() ?? false)
    {
        foreach (var item in data)
        {
            Console.WriteLine(item);
        }
    }

    Console.WriteLine("Consume Finish");
});

Console.ReadKey();