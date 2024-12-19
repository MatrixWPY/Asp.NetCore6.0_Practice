using StackExchange.Redis;
using static SubscribeMultiple.RedisHelper;

RedisConnection.Init("127.0.0.1:6379");
var redis = RedisConnection.Instance.ConnectionMultiplexer;
var db = redis.GetDatabase();

string channelName = "channel*";

var sub = redis.GetSubscriber();
sub.Subscribe(
    new RedisChannel(channelName, RedisChannel.PatternMode.Pattern),
    (chl, msg) =>
    {
        Console.WriteLine($"{DateTime.Now: yyyy/MM/dd HH:mm:ss.fff} - Received message : {msg}");
    }
);
Console.WriteLine($"{DateTime.Now: yyyy/MM/dd HH:mm:ss.fff} - Subscribed from channel : {channelName}");

Console.ReadKey();
